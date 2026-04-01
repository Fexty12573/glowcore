using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace GlowCore.World
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private NodeData m_nodeData;
        private float m_holdTimer;
        private float m_effectiveBreakTime;
        private bool m_isHolding;
        private IInteractable m_interactable;

        public List<Vector2Int> TilesUsed = new();
        public Outline Outline;
        public NodeData NodeData => m_nodeData;

        public float GetInteractionRange()
        {
            return m_nodeData.InteractionRange;
        }

        public void Interact()
        {
            m_interactable?.Interact();
        }

        public void StartHold(Tool tool)
        {
            m_effectiveBreakTime = m_nodeData.GetEffectiveBreakTime(tool);
            m_isHolding = true;
            m_holdTimer = 0f;
        }

        public void EndHold()
        {
            m_effectiveBreakTime = m_nodeData.BaseBreakTime;
            m_isHolding = false;
            m_holdTimer = 0f;
        }

        public void UpdateHold(float deltaTime)
        {
            if (!m_isHolding || m_nodeData.IsIndestructible)
                return;

            m_holdTimer += deltaTime;
            if (m_holdTimer >= m_effectiveBreakTime)
            {
                Break();
                m_isHolding = false;
            }
        }

        private void Awake()
        {
            TryGetComponent(out Outline);
            Collider[] colliders = GetComponentsInChildren<Collider>();
            HashSet<GameObject> processed = new HashSet<GameObject>();

            foreach (var col in colliders)
            {
                if (!processed.Add(col.gameObject))
                    continue;
                if (!col.TryGetComponent(out NodeActionChild child))
                {
                    child = col.gameObject.AddComponent<NodeActionChild>();
                }

                child.Root = this;
            }
        }

        private void Start()
        {
            TryGetComponent(out m_interactable);
        }

        private void Break()
        {
            foreach (var drop in m_nodeData.ItemDrops)
            {
                var offset = new Vector3(
                    Random.Range(-0.2f, 0.2f),
                    0f,
                    Random.Range(-0.2f, 0.2f));
                ItemStackDrop.Spawn(drop, transform.position + offset);
            }
            foreach (var tile in TilesUsed)
            {
                WorldGrid.Instance.ClearNodeAt(tile);
            }
            Destroy(gameObject);
        }
    }
}