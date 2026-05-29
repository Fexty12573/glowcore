using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Node m_node;
    [SerializeField] private Block m_rotatorBlock;

    private void Start() => m_node.SourceBlock = m_rotatorBlock;
}
