using GlowCore.UI.Menus;
using NUnit.Framework;

public class DescriptorRoundTripTests
{
    private FakePlayerPrefsBackend m_backend;
    private PlayerPrefsSettingsRepository m_repository;

    [SetUp]
    public void SetUp()
    {
        m_backend = new FakePlayerPrefsBackend();
        m_repository = new PlayerPrefsSettingsRepository(m_backend);
    }

    [Test]
    public void SliderDescriptor_RoundTripsThroughRepository()
    {
        SettingDescriptor descriptor = SettingDescriptor.Slider(
            "master", "Master", 0f, 1f,
            () => m_repository.GetFloat("master", 0.5f),
            v => m_repository.SetFloat("master", v));

        Assert.AreEqual(0.5f, System.Convert.ToSingle(descriptor.Read()));

        descriptor.Write(0.75f);

        Assert.AreEqual(0.75f, m_repository.GetFloat("master", 0f));
        Assert.AreEqual(0.75f, System.Convert.ToSingle(descriptor.Read()));
    }

    [Test]
    public void ToggleDescriptor_RoundTripsThroughRepository()
    {
        SettingDescriptor descriptor = SettingDescriptor.Toggle(
            "fullscreen", "Fullscreen",
            () => m_repository.GetBool("fullscreen", false),
            v => m_repository.SetBool("fullscreen", v));

        Assert.IsFalse(System.Convert.ToBoolean(descriptor.Read()));

        descriptor.Write(true);

        Assert.IsTrue(m_repository.GetBool("fullscreen", false));
        Assert.IsTrue(System.Convert.ToBoolean(descriptor.Read()));
    }

    [Test]
    public void DropdownDescriptor_RoundTripsThroughRepository()
    {
        string[] options = { "Low", "Med", "High" };
        SettingDescriptor descriptor = SettingDescriptor.Dropdown(
            "res", "Resolution", options,
            () => m_repository.GetInt("res", 1),
            v => m_repository.SetInt("res", v));

        Assert.AreEqual(1, System.Convert.ToInt32(descriptor.Read()));

        descriptor.Write(2);

        Assert.AreEqual(2, m_repository.GetInt("res", 0));
        Assert.AreEqual(2, System.Convert.ToInt32(descriptor.Read()));
    }

    [Test]
    public void SliderDescriptor_WriteAlsoTriggersSideEffect()
    {
        float lastBridgeValue = -1f;
        var bridge = new AudioBridge();
        bridge.OnMasterVolumeChanged += v => lastBridgeValue = v;

        SettingDescriptor descriptor = SettingDescriptor.Slider(
            "master", "Master", 0f, 1f,
            () => m_repository.GetFloat("master", 0.5f),
            v =>
            {
                m_repository.SetFloat("master", v);
                bridge.RaiseMaster(v);
            });

        descriptor.Write(0.4f);

        Assert.AreEqual(0.4f, m_repository.GetFloat("master", 0f));
        Assert.AreEqual(0.4f, lastBridgeValue);
    }

    [Test]
    public void DropdownDescriptor_HandlesEmptyOptions()
    {
        SettingDescriptor descriptor = SettingDescriptor.Dropdown(
            "empty", "Empty", new string[0],
            () => 0,
            _ => { });
        Assert.AreEqual(SettingControlKind.Dropdown, descriptor.Kind);
        Assert.AreEqual(0, descriptor.Options.Count);
    }
}
