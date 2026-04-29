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

    public static SaveData Load()
    {
        var saveData = Default();
        var path = GetPath();
        var reader = new BinaryReader(File.OpenRead(path));

        var magic = reader.ReadUInt32();
        if (magic != kMagic)
        {
            Debug.LogError($"Invalid save file: {path}");
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

    private static SaveData Default()
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

        Player.Name = Encoding.UTF8.GetString(reader.ReadBytes(32));
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
