namespace GlowCore.UI.Menus
{
    public class InventoryEscapeConsumer : IEscapeConsumer
    {
        public const int kPriority = 100;

        private readonly IInventoryService m_inventory;

        public InventoryEscapeConsumer(IInventoryService inventory)
        {
            m_inventory = inventory;
        }

        public int Priority => kPriority;

        public bool TryConsumeEscape()
        {
            if (m_inventory == null)
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
