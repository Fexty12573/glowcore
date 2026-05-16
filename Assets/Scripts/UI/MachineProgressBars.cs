using System.Collections.Generic;
using GlowCore.World;
using UnityEngine;
using UnityEngine.UI;

public class MachineProgressBars : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_progressBarPrefab;
    [SerializeField] private Vector3 m_offset;
    private Dictionary<Node, Slider> m_progressBars = new();

    private void Start()
    {
        Node.OnStartBreaking += HandleNodeStartBreaking;
        Node.OnCancelBreaking += HandleNodeEndBreaking;
        Node.OnNodeBroken += HandleNodeEndBreaking;
    }

    private void OnDestroy()
    {
        Node.OnStartBreaking -= HandleNodeStartBreaking;
        Node.OnCancelBreaking -= HandleNodeEndBreaking;
        Node.OnNodeBroken -= HandleNodeEndBreaking;
    }

    private void Update()
    {
        foreach ((Node node, Slider slider) in m_progressBars)
        {
            Vector3 promptPosition = node.transform.position + m_offset;
            slider.transform.position = m_camera.WorldToScreenPoint(promptPosition);
            slider.value = node.GetBreakProgress();
        }
    }

    private void HandleNodeStartBreaking(Node node, bool byPlayer)
    {
        if (byPlayer)
            return;

        Slider progressBar = Instantiate(m_progressBarPrefab, transform).GetComponent<Slider>();
        Vector3 promptPosition = node.transform.position + m_offset;
        progressBar.transform.position = m_camera.WorldToScreenPoint(promptPosition);
        progressBar.value = node.GetBreakProgress();
        m_progressBars.Add(node, progressBar);
    }

    private void HandleNodeEndBreaking(Node node)
    {
        if (!m_progressBars.TryGetValue(node, out Slider progressBar))
            return;

        Destroy(progressBar.gameObject);
        m_progressBars.Remove(node);
    }
}
