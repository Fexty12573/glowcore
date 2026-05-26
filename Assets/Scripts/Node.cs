using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BlockRotation
{
    North, East, South, West
}

namespace GlowCore.World
{
    public class Node : MonoBehaviour
    {
        [SerializeField] private NodeData m_nodeData;
        private float m_holdTimer;
        private float m_effectiveBreakTime;
        private bool m_isHolding;
        private bool m_markedForDeletion;
        private Chest m_breakOwner; // Chest is the storage of the machine that breaks this node, null if the player breaks it.
        private IInteractable m_interactable;

        public BlockRotation Rotation = BlockRotation.North;
        public List<Vector2Int> TilesUsed = new();
        public Outline Outline;
        public NodeData NodeData => m_nodeData;
        public Block SourceBlock { get; set; }

        public static event Action<Node, bool> OnStartBreaking; //second Argument tells if the player is the one who is breaking the Node.
        public static event Action<Node, bool> OnCancelBreaking;
        public static event Action<Node, bool> OnNodeBroken;

        public bool IsHolding => m_isHolding;
        public bool PlayerIsHolding => m_isHolding && m_breakOwner == null;
        public bool MarkedForDeletion => m_markedForDeletion;

        public static float BlockRotationToDegrees(BlockRotation rotation)
        {
            return rotation switch
            {
                BlockRotation.North => 0f,
                BlockRotation.East => 90f,
                BlockRotation.South => 180f,
                BlockRotation.West => 270f,
                _ => 0f
            };
        }

        public static Vector2Int RotationToDirectionVector2Int(BlockRotation rotation)
        {
            switch (rotation)
            {
                case BlockRotation.North:
                    return Vector2Int.up;
                case BlockRotation.East:
                    return Vector2Int.right;
                case BlockRotation.South:
                    return Vector2Int.down;
                case BlockRotation.West:
                    return Vector2Int.left;
                default:
                    return Vector2Int.zero;
            }
        }

        public static Vector3 RotationToDirectionVector3(BlockRotation rotation)
        {
            var vector = RotationToDirectionVector2Int(rotation);
            return new Vector3(vector.x, 0f, vector.y);
        }
        public float GetInteractionRange()
        {
            return m_nodeData.InteractionRange;
        }

        public void Interact()
        {
            m_interactable?.Interact();
        }

        public void StartHold(Tool tool, Chest machineStorage) //machineStorage is null if the player breaks it
        {
            m_effectiveBreakTime = m_nodeData.GetEffectiveBreakTime(tool);
            m_isHolding = true;
            m_holdTimer = 0f;

            AudioManager.Instance.Play(
                tool.Sound,
                AudioManager.AudioChannel.Environment);

            m_breakOwner = machineStorage;
            OnStartBreaking?.Invoke(this, (machineStorage is null));
        }

        public void EndHold()
        {
            m_effectiveBreakTime = m_nodeData.BaseBreakTime;
            m_isHolding = false;
            m_holdTimer = 0f;
            AudioManager.Instance.Stop(AudioManager.AudioChannel.Environment);
            OnCancelBreaking?.Invoke(this, (m_breakOwner is null));
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

        public float GetBreakProgress()
        {
            if (m_effectiveBreakTime <= 0)
                return 1;
            return m_holdTimer / m_effectiveBreakTime;
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
            if (m_breakOwner is null) //drop into world
            {
                foreach (var drop in m_nodeData.ItemDrops)
                    DropItems(drop);
            }
            else //move items into storage of machine
            {
                foreach (var drop in m_nodeData.ItemDrops)
                {
                    var amount = Random.Range(drop.Min, drop.Max + 1);
                    if (m_breakOwner.CanAcceptItem(drop.Item, amount))
                        m_breakOwner.AddStack(drop.Item, amount);
                    else //drop into world if it doesn't fit into chest
                        DropItems(drop);
                }
            }

            foreach (var tile in TilesUsed)
            {
                WorldGrid.Instance.ClearNodeAt(tile);
            }

            m_markedForDeletion = true;
            OnNodeBroken?.Invoke(this, (m_breakOwner is null));
            Destroy(gameObject);
            AudioManager.Instance.Stop(AudioManager.AudioChannel.Environment);
        }

        private void DropItems(ItemDrop drop)
        {
            var offset = new Vector3(
                Random.Range(-0.2f, 0.2f),
                0f,
                Random.Range(-0.2f, 0.2f));
            ItemStackDrop.Spawn(drop, transform.position + offset);
        }
    }
}