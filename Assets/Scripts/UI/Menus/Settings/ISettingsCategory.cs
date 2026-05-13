using System.Collections.Generic;

namespace GlowCore.UI.Menus
{
    public interface ISettingsCategory
    {
        string DisplayName { get; }
        IReadOnlyList<SettingDescriptor> Descriptors { get; }
    }
}
