using System.IO;
using NUnit.Framework;

public class SaveDataTests
{
    [Test]
    public void Default_HasCorrectValues()
    {
        var saveData = SaveData.Default();
        Assert.NotNull(saveData);
        Assert.NotNull(saveData.Player);
        Assert.AreEqual("Player", saveData.Player.Name);
        Assert.AreEqual(1, saveData.Player.GlowCoreLevel);
        Assert.AreEqual(0, saveData.Player.PosX);
        Assert.AreEqual(0, saveData.Player.PosZ);
        Assert.NotNull(saveData.Player.GlowCoreInventory);
        Assert.NotNull(saveData.Player.Inventory);
        Assert.NotNull(saveData.World);
        Assert.NotNull(saveData.World.TileDeltas);
        Assert.AreEqual(0, saveData.World.TileDeltas.Length);
    }

    [Test]
    public void SaveAndLoad_Stream_RetainsData()
    {
        // Arrange
        var original = SaveData.Default();
        original.Player.Name = "TestPlayer";
        original.Player.GlowCoreLevel = 5;
        original.Player.PosX = 10.5f;
        original.Player.PosZ = -20.3f;

        using var memoryStream = new MemoryStream();

        // Act
        SaveData.SaveTo(memoryStream, original);
        memoryStream.Position = 0;
        var loaded = SaveData.LoadFrom(memoryStream);

        // Assert
        Assert.NotNull(loaded);
        Assert.AreEqual(original.Player.Name, loaded.Player.Name);
        Assert.AreEqual(original.Player.GlowCoreLevel, loaded.Player.GlowCoreLevel);
        Assert.AreEqual(original.Player.PosX, loaded.Player.PosX);
        Assert.AreEqual(original.Player.PosZ, loaded.Player.PosZ);
    }

    [Test]
    public void LoadFrom_InvalidMagic_ReturnsDefault()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        writer.Write(0x12345678); // Invalid magic
        writer.Write(1);          // Version
        memoryStream.Position = 0;

        // Act
        var loaded = SaveData.LoadFrom(memoryStream);

        // Assert
        Assert.NotNull(loaded);
        Assert.AreEqual("Player", loaded.Player.Name); // default name
    }

    [Test]
    public void LoadFrom_InvalidVersion_ReturnsDefault()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        writer.Write(SaveData.kMagic);
        writer.Write(999); // Invalid version
        memoryStream.Position = 0;

        // Act
        var loaded = SaveData.LoadFrom(memoryStream);

        // Assert
        Assert.NotNull(loaded);
        Assert.AreEqual("Player", loaded.Player.Name); // default name
    }
}
