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
        if (m_itemsInHand.Item is null)
            return;
        m_itemGameObject = Instantiate(m_itemsInHand.Item.Prefab, m_playerHand);
        Rigidbody rb = m_itemGameObject.GetComponent<Rigidbody>();
        Destroy(rb);
        Outline outline = m_itemGameObject.GetComponent<Outline>();
        Destroy(outline);
    }

    private void OnUse(InputValue value)
    {
        var handItem = m_itemGameObject?.GetComponent<IHandItem>();
        if (handItem is null)
            return;
        handItem.Use(value);
    }
}
