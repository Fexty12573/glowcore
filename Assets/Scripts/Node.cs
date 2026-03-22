using System.Collections.Generic;
using ScriptableObjectScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlowCore.World
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private NodeData m_NodeData;
        private float m_holdTimer;
        private bool m_isHolding;
        private IInteractable m_Interactable;
        public Outline Outline;

        private void Awake()
        {
            TryGetComponent(out Outline);
            Collider[] colliders = GetComponentsInChildren<Collider>();
            HashSet<GameObject> processed = new HashSet<GameObject>();

            foreach (var col in colliders)
            {
                if (!processed.Add(col.gameObject)) continue;
                if (!col.TryGetComponent(out NodeActionChild child))
                {
                    child = col.gameObject.AddComponent<NodeActionChild>();
                }

                child.Root = this;
            }
        }


        private void Start()
        {
            TryGetComponent(out m_Interactable);
        }

        public float GetInteractRange()
        {
            return m_NodeData.InteractRange;
        }

        public void Interact()
        {
            m_Interactable?.Interact();
        }

        private void Break()
        {
            foreach (var drop in m_NodeData.ItemDrops)
            {
                int amount = Random.Range(drop.min, drop.max + 1);
                for (int i = 0; i < amount; i++)
                {
                    Vector3 spawnPos = transform.position +
                                       new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
                    Instantiate(drop.item.Prefab, spawnPos, Quaternion.identity);
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
            if (!m_isHolding || m_NodeData.IsIndestructible) return;

            m_holdTimer += deltaTime;
            if (m_holdTimer >= m_NodeData.BreakTime)
            {
                Break();
                m_isHolding = false;
            }
        }
    }
}