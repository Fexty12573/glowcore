using ScriptableObjects;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private const int kWidth = 8;
    private const int kHeight = 4;

    [SerializeField] private Inventory m_inventory;

    private void Awake() => m_inventory = new Inventory(kWidth, kHeight);

    public void Add(ItemStack stack) => m_inventory.AddItems(stack);
}
