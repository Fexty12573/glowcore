using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory s_instance;

    private const int kWidth = 8;
    private const int kHeight = 4;

    private GameObject m_handItemGameObject;
    private int m_hotbarIndex = 0;
    [SerializeField] private Inventory m_inventory;

    public static PlayerInventory Instance => s_instance;

    public ItemStack ItemsInHand => m_inventory[m_hotbarIndex, 0];
    public void Add(ItemStack stack) => m_inventory.AddItems(stack);

    public bool ConsumeItemInHand(int amount) => m_inventory.RemoveItemsAt(m_hotbarIndex, 0, amount);

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError("PlayerInventory: Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        m_inventory = new Inventory(kWidth, kHeight);
        Inventory.OnInventoryChange += UpdateHandGameObject;
    }

    private void OnDestroy() => Inventory.OnInventoryChange -= UpdateHandGameObject;

    private void UpdateHandGameObject()
    {
        Destroy(m_handItemGameObject);
        m_handItemGameObject = null;
        ItemStack itemsInHand = m_inventory[m_hotbarIndex, 0];
        if (itemsInHand?.Item is null)
            return;

        m_handItemGameObject = Instantiate(itemsInHand.Item.Prefab, transform);
        m_handItemGameObject.transform.localScale *= itemsInHand.Item.InHandScale;
        Rigidbody rb = m_handItemGameObject.GetComponent<Rigidbody>();
        Destroy(rb);
        Outline outline = m_handItemGameObject.GetComponent<Outline>();
        Destroy(outline);
        ActivateHandItem();
    }

    private void ActivateHandItem()
    {
        IHandItem handItem = m_handItemGameObject?.GetComponent<IHandItem>();
        if (handItem is not null)
            (handItem as MonoBehaviour).enabled = true;
    }

    private void OnUse(InputValue value)
    {
        IHandItem handItem = m_handItemGameObject?.GetComponent<IHandItem>();
        handItem?.Use(value);
    }


    private void OnPrevious(InputValue value)
    {
        m_hotbarIndex = (m_hotbarIndex - 1 + kWidth) % kWidth;
        UpdatePlayerHand();
    }

    private void OnNext(InputValue value)
    {
        m_hotbarIndex = (m_hotbarIndex + 1) % kWidth;
        UpdatePlayerHand();
    }

    private void UpdatePlayerHand()
    {
        UpdateHandGameObject();
        // This ensures that the HandItem is notified at the start of its lifetime about the hovered Node/Tile
        // without the need to move the mouse.
        NodeActionSystem.OnHandItemChange();
    }
}