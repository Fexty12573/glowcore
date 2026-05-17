using System.Collections.Generic;
using GlowCore.UI.Menus;

public class FakeSettingsCategory : ISettingsCategory
{
    public string DisplayName { get; set; } = "FAKE";
    public IReadOnlyList<SettingDescriptor> Descriptors { get; }

    public FakeSettingsCategory(IReadOnlyList<SettingDescriptor> descriptors)
    {
        Descriptors = descriptors;
    }
}
