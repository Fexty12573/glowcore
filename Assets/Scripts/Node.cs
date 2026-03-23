using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlowCore.World
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private NodeData m_nodeData;
        private float m_holdTimer;
        private bool m_isHolding;
        private IInteractable m_interactable;
        public Outline Outline;

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

        public float GetInteractRange()
        {
            return m_nodeData.InteractRange;
        }

        public void Interact()
        {
            m_interactable?.Interact();
        }

        private void Break()
        {
            foreach (var drop in m_nodeData.ItemDrops)
            {
                int amount = Random.Range(drop.Min, drop.Max + 1);
                for (var i = 0; i < amount; i++)
                {
                    Vector3 spawnPos = transform.position +
                                       new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
                    Instantiate(drop.Item.Prefab, spawnPos, Quaternion.identity);
                }
            }

            Destroy(gameObject);
        }

        public void StartHold()
        {
            m_isHolding = true;
            m_holdTimer = 0f;
        }

        public void EndHold()
        {
            m_isHolding = false;
            m_holdTimer = 0f;
        }

        public void UpdateHold(float deltaTime)
        {
            if (!m_isHolding || m_nodeData.IsIndestructible)
                return;

            m_holdTimer += deltaTime;
            if (m_holdTimer >= m_nodeData.BreakTime)
            {
                Break();
                m_isHolding = false;
            }
        }
    }
}