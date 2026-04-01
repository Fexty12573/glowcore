using System;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
public class ItemStackDrop : MonoBehaviour
{
    private const float kVisualSpread = 0.1f;
    private const float kBobSpeed = 3f;
    private const float kBobAmplitude = 0.05f;
    private const float kGroundOffset = 0.1f;
    private const float kPickupRadius = 0.5f;

    public ItemStack Stack { get; private set; }

    private float m_bobPhase;
    private Vector3 m_baseLocalPos;
    private SphereCollider m_pickupTrigger;
    private bool m_collected;

    private void Awake()
    {
        m_pickupTrigger = GetComponent<SphereCollider>();
        m_pickupTrigger.isTrigger = true;
        m_pickupTrigger.radius = kPickupRadius;
    }

    private void OnValidate()
    {
        if (m_pickupTrigger == null)
            m_pickupTrigger = GetComponent<SphereCollider>();

        if (m_pickupTrigger != null)
        {
            m_pickupTrigger.isTrigger = true;
            m_pickupTrigger.radius = kPickupRadius;
        }
    }

    public static ItemStackDrop Spawn(ItemDrop drop, Vector3 position)
    {
        if (drop == null)
            return null;

        var amount = Random.Range(drop.Min, drop.Max + 1);

        var dropRoot = new GameObject($"{drop.Item.Name}_x{amount}_Stack")
        {
            transform = { position = position }
        };

        var stackDrop = dropRoot.AddComponent<ItemStackDrop>();
        stackDrop.Initialize(drop.Item, amount);

        return stackDrop;
    }

    public static ItemStackDrop Spawn(ItemStack itemStack, Vector3 position)
    {
        ItemDrop drop = new() { Item = itemStack.Item, Min = itemStack.Amount, Max = itemStack.Amount };
        return Spawn(drop, position);
    }

    public void Initialize(Item item, int amount)
    {
        if (item == null || amount == 0)
            return;

        Stack = new ItemStack(item, amount);

        BuildVisuals();
    }

    private void BuildVisuals()
    {
        if (!Stack.IsValid)
            return;

        var visibleAmount = Math.Clamp(Stack.Amount, 1, 3);
        for (var i = 0; i < visibleAmount; i++)
        {
            var offset = new Vector3(
                Random.Range(-kVisualSpread, kVisualSpread),
                0f,
                Random.Range(-kVisualSpread, kVisualSpread));

            var visual = Instantiate(Stack.Item.Prefab, transform);
            visual.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
            visual.transform.localScale *= Stack.Item.DropScale;

            RemoveCollisions(visual);
        }
    }

    private static void RemoveCollisions(GameObject obj)
    {
        foreach (var collider in obj.GetComponentsInChildren<Collider>())
            collider.enabled = false;

        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }

    private void Start()
    {
        m_baseLocalPos = transform.localPosition;
        m_baseLocalPos.y += kGroundOffset;

        m_bobPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void FixedUpdate()
    {
        var height = Mathf.Sin(m_bobPhase + (Time.time * kBobSpeed)) * kBobAmplitude;
        var pos = m_baseLocalPos;
        pos.y += height;
        transform.localPosition = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_collected || !Stack.IsValid)
            return;

        var root = other.transform.root;
        var inventory = other.GetComponentInParent<PlayerInventory>() ??
                        root.GetComponentInChildren<PlayerInventory>(true);

        if (inventory == null)
            return;

        inventory.Add(Stack);
        m_collected = true;
        Destroy(gameObject);
    }
}
