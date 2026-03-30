using ScriptableObjects;
using UnityEngine;

namespace GlowCore.World
{
    public class GlowCoreObject : MonoBehaviour, IInteractable
    {
        // Instance Fields
        [SerializeField] private GameObject[] m_logs;
        [SerializeField] private GameObject m_nextLevelPrefab;
        [SerializeField][Range(0, 17)] private int m_initialActiveLogs = 3;
        [SerializeField][Min(1)] private int m_startingLevel = 1;
        [SerializeField][Min(1)] private int m_woodToLevelUp = 17;
        [SerializeField] private PlayerInventory m_playerInventory;
        private int m_activeLogs;
        private int m_level;
        private int m_woodAccumulated;

        // Properties
        public int Level => m_level;
        public int WoodAccumulated => m_woodAccumulated;
        public int WoodToLevelUp => m_woodToLevelUp;

        // Public Methods
        public void Interact()
        {
            ItemStack itemsInHand = m_playerInventory.ItemsInHand;
            if (itemsInHand?.Item is null)
                return;

            if (itemsInHand.Item.Name == "Wood")
            {
                int woodAmount = itemsInHand.Amount;
                if (!m_playerInventory.ConsumeItemInHand(woodAmount))
                {
                    Debug.LogError("Failed to consume wood to feed GlowCore.");
                    return;
                }
                gameObject.TryGetComponent(out Fire fire);
                fire?.FeedWood(woodAmount);
            }
        }

        public bool FeedWood(int amount)
        {
            ActivateLogs(amount);

            if (m_woodAccumulated < m_woodToLevelUp)
                return false;

            m_woodAccumulated -= m_woodToLevelUp;
            m_level++;
            Debug.Log($"GlowCore leveled up to level {m_level}!");
            UpgradeGlowCore();
            return true;
        }

        public void ActivateLogs(int amount)
        {
            if (m_activeLogs >= m_logs.Length)
                return;

            var toActivate = Mathf.Min(amount, m_logs.Length - m_activeLogs);
            for (var i = 0; i < toActivate; i++)
            {
                GameObject log = m_logs[m_activeLogs];
                log.SetActive(true);
                RegisterInteractableChildren(log);
                m_activeLogs++;
                m_woodAccumulated++;
            }

            Debug.Log($"GlowCore: Activated {toActivate} log(s). Total active: {m_activeLogs}/{m_logs.Length}.");

            if (TryGetComponent(out Outline outline))
                outline.RefreshRenderers();
        }

        // Private Methods
        private void Awake()
        {
            m_level = m_startingLevel;

            foreach (GameObject log in m_logs)
                log.SetActive(false);

            ActivateLogs(m_initialActiveLogs);
        }

        private void UpgradeGlowCore()
        {
            if (m_nextLevelPrefab == null)
            {
                Debug.Log("GlowCore: Max level reached, no upgrade available.");
                return;
            }

            Vector3 position = transform.position;
            var worldX = Mathf.RoundToInt(position.x);
            var worldZ = Mathf.RoundToInt(position.z);

            WorldGrid.Instance.PlaceNodeAt(null, worldX, worldZ);

            GameObject newGlowCore = Instantiate(m_nextLevelPrefab, position, Quaternion.identity);

            if (newGlowCore.TryGetComponent(out Node newNode))
            {
                WorldGrid.Instance.PlaceNodeAt(null, worldX, worldZ);
            }
            else
            {
                Debug.LogError("GlowCore: Level prefab is missing a Node component.");
            }

            Destroy(gameObject);
        }

        private void RegisterInteractableChildren(GameObject target)
        {
            Node root = GetComponentInParent<Node>();
            if (root == null)
                return;

            foreach (Collider col in target.GetComponentsInChildren<Collider>())
            {
                if (!col.TryGetComponent(out NodeActionChild child))
                    child = col.gameObject.AddComponent<NodeActionChild>();

                child.Root = root;
            }
        }
    }
}