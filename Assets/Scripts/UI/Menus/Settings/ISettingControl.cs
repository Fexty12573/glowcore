namespace GlowCore.UI.Menus
{
    public interface ISettingControl
    {
        void Bind(SettingDescriptor descriptor);
        void Unbind();
    }
}
