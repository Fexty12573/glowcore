using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    private const int kWidth = 8;
    private const int kHeight = 4;

    [SerializeField] private Inventory m_inventory;
    [SerializeField] private PlayerHand m_playerHand;
    private int m_hotbarIndex = 0;

    private void Awake() => m_inventory = new Inventory(kWidth, kHeight);

    public void Add(ItemStack stack) => m_inventory.AddItems(stack);

    private void OnPrevious(InputValue value)
    {
        m_hotbarIndex = (m_hotbarIndex + 1) % kWidth;
        UpdatePlayerHand();
    }

    private void OnNext(InputValue value)
    {
        m_hotbarIndex = (m_hotbarIndex - 1 + kWidth) % kWidth;
        UpdatePlayerHand();
    }

    private void UpdatePlayerHand()
    {
        m_playerHand.SetItemInHand(m_inventory[m_hotbarIndex, 0]);
    }
}
