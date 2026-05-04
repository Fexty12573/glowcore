using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using ScriptableObjects;
using UnityEngine;

public class SaveData
{
    public const uint kMagic = 0x56534347; // "GCSV" in ASCII
    public const int kVersion = 1;
    private const string kFileName = "glowcore.bin";

    public PlayerData Player { get; private set; }
    public WorldData World { get; private set; }

    public static string GetPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var saveDataDir = Path.Combine(appData, "GlowCore", "savedata");
        Directory.CreateDirectory(saveDataDir);

        return Path.Combine(saveDataDir, kFileName);
    }

    public static bool Exists() => File.Exists(GetPath());

    public static SaveData Load()
    {
        using var stream = File.OpenRead(GetPath());
        return LoadFrom(stream);
    }

    public static void Save(SaveData saveData)
    {
        using var stream = File.OpenWrite(GetPath());
        SaveTo(stream, saveData);
    }

    public static SaveData LoadFrom(Stream stream)
    {
        var saveData = Default();
        var reader = new BinaryReader(stream);

        var magic = reader.ReadUInt32();
        if (magic != kMagic)
        {
            Debug.LogError($"Invalid save file. Magic is {magic:X8}, should be {kMagic:X8}");
            return saveData;
        }

        var version = reader.ReadInt32();
        switch (version)
        {
            case 1:
                saveData.LoadV1(reader);
                break;
            default:
                Debug.LogError($"Unsupported save version: {version}");
                return saveData;
        }

        return saveData;
    }

    public static void SaveTo(Stream stream, SaveData saveData)
    {
        var writer = new BinaryWriter(stream);

        writer.Write(kMagic);
        writer.Write(kVersion);

        saveData.Save(writer);
    }

    public static SaveData Default()
    {
        var saveData = new SaveData
        {
            Player = new PlayerData
            {
                Name = "Player",
                GlowCoreLevel = 1,
                PosX = 0,
                PosZ = 0,
                GlowCoreInventory = new Inventory(1, 1),
                Inventory = new Inventory(8, 4),
            },
            World = new WorldData { TileDeltas = Array.Empty<TileDelta>(), }
        };

        return saveData;
    }

    private void LoadV1(BinaryReader reader)
    {
        var playerDataOffset = reader.ReadUInt32();
        var worldDataOffset = reader.ReadUInt32();

        // Load Player Data
        reader.BaseStream.Seek(playerDataOffset, SeekOrigin.Begin);

        Player.Name = Encoding.UTF8.GetString(reader.ReadBytes(32)).TrimEnd('\0');
        Player.GlowCoreLevel = reader.ReadUInt16();
        Player.PosX = reader.ReadSingle();
        Player.PosZ = reader.ReadSingle();
        Player.GlowCoreInventory = LoadInventory(reader);
        Player.Inventory = LoadInventory(reader);

        // Load World Data
        reader.BaseStream.Seek(worldDataOffset, SeekOrigin.Begin);

        var tileDeltaCount = reader.ReadInt32();
        var firstTileDeltaOffset = reader.ReadUInt32();
        World.TileDeltas = new TileDelta[tileDeltaCount];

        reader.BaseStream.Seek(firstTileDeltaOffset, SeekOrigin.Begin);

        for (var i = 0; i < tileDeltaCount; i++)
            World.TileDeltas[i] = LoadTileDelta(reader);
    }

    private void Save(BinaryWriter writer)
    {
        writer.Write(0); // Player Data Offset
        writer.Write(0); // World Data Offset

        var playerOffset = writer.BaseStream.Position;

        var nameBytes = Encoding.UTF8.GetBytes(Player.Name);
        if (nameBytes.Length != 32)
            Array.Resize(ref nameBytes, 32);

        writer.Write(nameBytes);
        writer.Write(Player.GlowCoreLevel);
        writer.Write(Player.PosX);
        writer.Write(Player.PosZ);
        SaveInventory(Player.GlowCoreInventory, writer);
        SaveInventory(Player.Inventory, writer);

        AlignTo(writer, 16);

        var worldOffset = writer.BaseStream.Position;

        writer.Write(World.TileDeltas.Length);
        writer.Write(0);

        AlignTo(writer, 16);

        var firstTileOffset = writer.BaseStream.Position;

        foreach (var delta in World.TileDeltas)
        {
            SaveTileDelta(writer, delta);
        }

        // Write offsets
        writer.BaseStream.Seek(8, SeekOrigin.Begin);
        writer.Write((uint)playerOffset);
        writer.Write((uint)worldOffset);

        writer.BaseStream.Seek(worldOffset + 4, SeekOrigin.Begin);
        writer.Write((uint)firstTileOffset);
    }

    private TileDelta LoadTileDelta(BinaryReader reader)
    {
        var type = (DeltaType)reader.ReadByte();
        var x = reader.ReadInt32();
        var z = reader.ReadInt32();

        var delta = new TileDelta { Type = type, X = x, Z = z, };

        if (type != DeltaType.Build)
            return delta;

        var id = new Guid(reader.ReadBytes(16));
        if (id == Guid.Empty)
        {
            Debug.LogWarning($"Empty build tile delta at ({x}, {z}), ignoring");
            return delta;
        }

        var item = ItemRegistry.Instance.Lookup(id);
        if (item is Block block)
        {
            delta.BuildData = new BuildData { Block = block };

            // TODO: Add a HasInventory field or something to `Block`
            if (block.Name == "Chest")
                delta.BuildData.Inventory = LoadInventory(reader);
        }
        else
        {
            Debug.LogWarning($"Non-block item `{item.Name}` found as build delta, ignoring");
        }

        return delta;
    }

    private static void SaveTileDelta(BinaryWriter writer, TileDelta delta)
    {
        writer.Write((byte)delta.Type);
        writer.Write(delta.X);
        writer.Write(delta.Z);

        if (delta.Type == DeltaType.Build)
        {
            writer.Write(delta.BuildData.Block.Id.ToByteArray());

            if (delta.BuildData.Inventory != null)
                SaveInventory(delta.BuildData.Inventory, writer);
        }
    }

    private static Inventory LoadInventory(BinaryReader reader)
    {
        var width = (int)reader.ReadUInt16();
        var height = (int)reader.ReadUInt16();

        var inventory = new Inventory(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var itemId = new Guid(reader.ReadBytes(16));
                var quantity = reader.ReadInt32();

                if (itemId == Guid.Empty)
                    continue;

                var item = ItemRegistry.Instance.Lookup(itemId);
                if (item == null)
                {
                    Debug.LogError($"Could not find item with id {itemId}");
                    continue;
                }

                inventory[x, y] = new ItemStack(item, quantity);
            }
        }

        return inventory;
    }

    private static void SaveInventory(Inventory inventory, BinaryWriter writer)
    {
        writer.Write((ushort)inventory.Width);
        writer.Write((ushort)inventory.Height);

        for (var y = 0; y < inventory.Height; y++)
        {
            for (var x = 0; x < inventory.Width; x++)
            {
                ref var stack = ref inventory[x, y];
                if (stack.IsValid)
                {
                    writer.Write(stack.Item.Id.ToByteArray());
                    writer.Write(stack.Amount);
                }
                else
                {
                    writer.Write(Guid.Empty.ToByteArray());
                    writer.Write(0);
                }
            }
        }
    }

    private static void AlignTo(BinaryWriter writer, long bytes)
    {
        while (writer.BaseStream.Position % bytes != 0)
            writer.Write((byte)0);
    }
}

public class PlayerData
{
    public string Name;
    public ushort GlowCoreLevel;
    public float PosX;
    public float PosZ;
    public Inventory GlowCoreInventory;
    public Inventory Inventory;
}

public class WorldData
{
    public TileDelta[] TileDeltas;
}

public class TileDelta
{
    public DeltaType Type;
    public int X;
    public int Z;
    [CanBeNull] public BuildData BuildData;
}

public class BuildData
{
    public Block Block;
    [CanBeNull] public Inventory Inventory;
}

public enum DeltaType : byte
{
    Break = 0,
    Build = 1,
}
