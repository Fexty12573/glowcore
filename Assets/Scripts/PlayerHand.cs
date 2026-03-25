using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IHandItem
{
    void Use(InputValue value);
}

public class PlayerHand : MonoBehaviour
{
    private static PlayerHand s_instance;
    
    [SerializeField] private Transform m_playerHand;
    private ItemStack m_itemsInHand;
    private GameObject itemGameObject;

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
        Destroy(itemGameObject);
        if (m_itemsInHand.Item is null)
            return;
        itemGameObject = Instantiate(m_itemsInHand.Item.Prefab, m_playerHand);
        Rigidbody rb = itemGameObject.GetComponent<Rigidbody>();
        Destroy(rb);
        Outline outline = itemGameObject.GetComponent<Outline>();
        Destroy(outline);
    }
    
    private void OnUse(InputValue value)
    {
        var handItem = itemGameObject?.GetComponent<IHandItem>();
        if (handItem is null)
            return;
        handItem.Use(value);
    }
}
