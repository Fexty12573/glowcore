namespace GlowCore.UI.Menus
{
    public class InventoryEscapeConsumer : IEscapeConsumer
    {
        public const int kPriority = 100;

        private readonly IInventoryService m_inventory;
        private readonly IMenuManager m_menuManager;

        public InventoryEscapeConsumer(IInventoryService inventory, IMenuManager menuManager)
        {
            m_inventory = inventory;
            m_menuManager = menuManager;
        }

        public int Priority => kPriority;

        public bool TryConsumeEscape()
        {
            if (m_inventory == null)
                return false;

            // Defer to menu consumers when a screen is layered on top — closing the
            // pause/settings/dialog must come before closing the inventory beneath it.
            if (m_menuManager != null && m_menuManager.Current != null)
                return false;

            var anyOpen = m_inventory.IsOpen
                          || m_inventory.IsCraftingStationOpen
                          || m_inventory.IsGlowCoreUIOpen
                          || m_inventory.IsChestOpen;
            if (!anyOpen)
                return false;

            // Single entry point that closes inventory directly and fires OnCloseUIRequested,
            // letting crafting / glowcore / future panels close themselves via the existing event.
            m_inventory.RequestCloseUI();
            return true;
        }
    }
}
