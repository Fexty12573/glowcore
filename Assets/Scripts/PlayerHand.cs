using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    private static PlayerHand s_instance;

    [SerializeField] private PlayerInventory m_playerInventory;

    private ItemStack m_itemsInHand;
    private GameObject m_itemGameObject;

    public static PlayerHand Instance => s_instance;
    public ItemStack ItemsInHand => m_itemsInHand;

    private void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogWarning("PlayerHand: Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        s_instance = this;
    }

    public void SetItemInHand(ItemStack itemStack)
    {
        m_itemsInHand = itemStack;
        UpdateHandVisual();
    }

    public void UpdateHandVisual()
    {
        Destroy(m_itemGameObject);
        m_itemGameObject = null;
        if (m_itemsInHand?.Item is null)
            return;
        m_itemGameObject = Instantiate(m_itemsInHand.Item.Prefab, transform);
        m_itemGameObject.transform.localScale *= m_itemsInHand.Item.InHandScale;
        Rigidbody rb = m_itemGameObject.GetComponent<Rigidbody>();
        Destroy(rb);
        Outline outline = m_itemGameObject.GetComponent<Outline>();
        Destroy(outline);
        if (m_itemGameObject.TryGetComponent(out IHandItem handItem) && handItem is MonoBehaviour bhv)
            bhv.enabled = true;
        if (m_itemGameObject.TryGetComponent(out IPlayerInventoryAware inventoryAware))
            inventoryAware.SetInventory(m_playerInventory);
    }

    private void OnUse(InputValue value)
    {
        if (m_playerInventory != null && m_playerInventory.IsAnyUIOpen)
            return;
        var handItem = m_itemGameObject?.GetComponent<IHandItem>();
        if (handItem is null)
            return;
        handItem.Use(value);
    }

    private void OnRotate(InputValue value)
    {
        if (m_playerInventory != null && m_playerInventory.IsAnyUIOpen)
            return;
        var block = m_itemGameObject?.GetComponent<BlockBehaviour>();
        if (block is null)
            return;
        block.Rotate(value);
    }
}