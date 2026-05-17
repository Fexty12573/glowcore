namespace GlowCore.UI.Menus
{
    public interface IEscapeConsumer
    {
        int Priority { get; }
        bool TryConsumeEscape();
    }
}
