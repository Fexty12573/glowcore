using System.Collections;
using System.Collections.Generic;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Sapling : MonoBehaviour
{
    [SerializeField] private Node m_node;
    [SerializeField] private Block[] m_canGrowIntoBlocks;
    [SerializeField] private float m_minGrowthTime = 4f;
    [SerializeField] private float m_maxGrowthTime = 7f;

    private void Start()
    {
        StartCoroutine(WaitGrow());
    }

    private IEnumerator WaitGrow()
    {
        float growthTime = Random.Range(m_minGrowthTime, m_maxGrowthTime);
        yield return new WaitForSeconds(growthTime);
        FinishGrow();
    }

    private void FinishGrow()
    {
        Block blockToBuild = m_canGrowIntoBlocks[Random.Range(0, m_canGrowIntoBlocks.Length)];
        WorldGrid.Instance.ReplaceNode(m_node, blockToBuild, false);
    }
}
