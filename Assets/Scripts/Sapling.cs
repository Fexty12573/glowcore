using System.Collections;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

public class Sapling : MonoBehaviour
{
    [SerializeField] private Node m_node;
    [SerializeField] private Block m_grownUpTree;
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
        WorldGrid.Instance.ReplaceNode(m_node, m_grownUpTree, false);
    }
}
