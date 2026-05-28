using System.Collections;
using System.Collections.Generic;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Sapling : MonoBehaviour
{
    [SerializeField] private Node m_node;
    [SerializeField] private Block[] m_canGrowIntoBlocks;
    [SerializeField] private float m_growthTime = 4f;

    private void Start()
    {
        StartCoroutine(WaitGrow());
    }

    private IEnumerator WaitGrow()
    {
        yield return new WaitForSeconds(m_growthTime);
        FinishGrow();
    }

    private void FinishGrow()
    {
        Block blockToBuild = m_canGrowIntoBlocks[Random.Range(0, m_canGrowIntoBlocks.Length)];
        WorldGrid.Instance.ReplaceNode(m_node, blockToBuild, false);
    }
}
