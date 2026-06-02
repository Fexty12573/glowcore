using System;
using System.Collections.Generic;
using GlowCore.UI.Upgrade;
using ScriptableObjects;
using UnityEngine;

namespace GlowCore.World
{
    public class GlowCoreObject : MonoBehaviour, IInteractable, IGlowCoreObject
    {
        [Header("Level")][SerializeField] private GlowCoreLevelConfig m_levelConfig;
        [SerializeField] private GameObject m_nextLevelPrefab;

        [Header("References")]
        [SerializeField]
        [Min(0f)]
        private float m_closeDistance = 5f;

        private Inventory m_glowCoreInventory;
        private int m_bankedForExpansion;
        private GlowCoreUpgradeUI m_ui;
        private PlayerInventory m_playerInventory;

        // Properties
        public GlowCoreLevelConfig LevelConfig => m_levelConfig;

        public GlowCoreLevelConfig NextLevelConfig
        {
            get
            {
                if (m_nextLevelPrefab == null)
                    return null;
                GlowCoreObject next = m_nextLevelPrefab.GetComponent<GlowCoreObject>();
                return next?.m_levelConfig;
            }
        }

        public int Level => m_levelConfig != null ? m_levelConfig.Level : 1;
        public bool HasNextLevel => m_nextLevelPrefab != null;
        public bool IsReadyToUpgrade => AreAllMaterialsMet();
        public Inventory GetInventory() => m_glowCoreInventory;

        public float TotalProgress01
        {
            get
            {
                if (m_levelConfig == null)
                    return 0f;

                IReadOnlyList<Recipe.Ingredient> materials = m_levelConfig.RequiredMaterials;
                if (materials.Count == 0)
                    return 0f;

                var totalRequired = 0;
                var totalAccumulated = 0;
                for (var i = 0; i < materials.Count; i++)
                {
                    totalRequired += materials[i].Amount;
                    totalAccumulated += Mathf.Min(AccumulatedFor(materials[i].Item), materials[i].Amount);
                }

                return totalRequired == 0 ? 0f : (float)totalAccumulated / totalRequired;
            }
        }

        // Events
        public event Action OnProgressChanged;
        public event Action OnLevelUp;

        // Raised once when a player upgrade brings the GlowCore to its final level.
        // Static because each upgrade destroys this instance and spawns the next level's object,
        // so listeners (e.g. the ending sequence) cannot bind to a specific instance.
        public static event Action OnEndingReached;

        // Public Methods
        public int AccumulatedFor(Item item)
        {
            if (item == null)
                return 0;
            return m_glowCoreInventory.CountItem(item);
        }

        public int RequiredFor(Item item)
        {
            if (item == null || m_levelConfig == null)
                return 0;

            IReadOnlyList<Recipe.Ingredient> materials = m_levelConfig.RequiredMaterials;
            for (var i = 0; i < materials.Count; i++)
            {
                if (materials[i].Item == item)
                    return materials[i].Amount;
            }

            return 0;
        }

        public void Interact()
        {
            if (m_ui == null)
                m_ui = FindFirstObjectByType<GlowCoreUpgradeUI>(FindObjectsInactive.Include);

            if (m_ui == null)
            {
                Debug.LogError("GlowCoreObject: No GlowCoreUpgradeUI found in scene.");
                return;
            }

            m_ui.Show(this);
        }

        public string GetActionPromptText() => "Feed GlowCore";

        public void FeedMaterial(Item item, int amount)
        {
            if (item == null || amount <= 0 || m_levelConfig == null)
                return;

            var required = RequiredFor(item);
            if (required <= 0)
                return;

            var accumulated = AccumulatedFor(item);
            var stillNeeded = required - accumulated;
            if (stillNeeded <= 0)
                return;

            var available = m_playerInventory != null ? m_playerInventory.CountItem(item) : 0;
            var consume = Mathf.Min(amount, Mathf.Min(stillNeeded, available));
            if (consume <= 0)
                return;

            m_playerInventory.RemoveItems(item, consume);
            m_glowCoreInventory.AddItems(new(item, consume));

            OnProgressChanged?.Invoke();
        }

        public void Upgrade()
        {
            if (!AreAllMaterialsMet())
                return;

            OnLevelUp?.Invoke();
            UpgradePhysical();
            GlowCoreObject next = SpawnNextLevel();
            AudioManager.Instance.PlayOneShot(AudioManager.SoundType.GlowCoreUpgrade,
                AudioManager.AudioChannel.Environment);

            if (next != null && !next.HasNextLevel)
                OnEndingReached?.Invoke();
        }

        public GlowCoreObject ForceUpgrade()
        {
            if (!HasNextLevel)
                return null;

            UpgradePhysical();
            return SpawnNextLevel();
        }

        // Private Methods
        private void Awake()
        {
            m_glowCoreInventory = new Inventory(8, 32);
            m_playerInventory = FindFirstObjectByType<PlayerInventory>();

            RenderSettings.sun.intensity += 0.02f;
        }

        private void Update()
        {
            if (m_ui == null || !m_ui.IsVisible || m_playerInventory == null)
                return;

            Vector3 playerPos = m_playerInventory.transform.position;
            Vector3 selfPos = transform.position;
            playerPos.y = 0f;
            selfPos.y = 0f;

            if (Vector3.Distance(playerPos, selfPos) > m_closeDistance)
                m_ui.Hide();
        }

        private void UpgradePhysical()
        {
            if (WorldGrid.Instance != null && m_levelConfig != null)
                WorldGrid.Instance.Expand(m_levelConfig.TilesOnLevelUp, isLevelUp: true);
        }

        private bool AreAllMaterialsMet()
        {
            if (m_levelConfig == null)
                return false;

            IReadOnlyList<Recipe.Ingredient> materials = m_levelConfig.RequiredMaterials;
            if (materials.Count == 0)
                return false;

            for (var i = 0; i < materials.Count; i++)
            {
                if (AccumulatedFor(materials[i].Item) < materials[i].Amount)
                    return false;
            }

            return true;
        }

        private GlowCoreObject SpawnNextLevel()
        {
            if (m_nextLevelPrefab == null)
            {
                Debug.Log("GlowCore: Max level reached, no upgrade available.");
                return null;
            }

            Vector3 position = transform.position;
            var worldX = Mathf.RoundToInt(position.x);
            var worldZ = Mathf.RoundToInt(position.z);

            if (TryGetComponent(out Node oldNode))
            {
                for (var i = 0; i < oldNode.TilesUsed.Count; i++)
                    WorldGrid.Instance.ClearNodeAt(oldNode.TilesUsed[i]);
            }

            GameObject newGlowCoreObj = Instantiate(m_nextLevelPrefab, position, Quaternion.identity, transform.parent);
            var newGlowCore = newGlowCoreObj.GetComponent<GlowCoreObject>();

            if (newGlowCoreObj.TryGetComponent(out Node newNode))
            {
                var nextConfig = newGlowCore?.LevelConfig;
                var tileCount = nextConfig != null ? nextConfig.TileCount : 1;
                WorldGrid.Instance.PlaceNodeAt(newNode, worldX, worldZ, tileCount);
            }
            else
            {
                Debug.LogError("GlowCore: Level prefab is missing a Node component.");
            }

            Destroy(gameObject);
            return newGlowCore;
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