using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Upgrade
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Image m_fillImage;
        [SerializeField] private TextMeshProUGUI m_percentLabel;

        public void SetProgress(float progress01)
        {
            var clamped = Mathf.Clamp01(progress01);
            if (m_fillImage != null)
                m_fillImage.fillAmount = clamped;
            if (m_percentLabel != null)
                m_percentLabel.text = Mathf.RoundToInt(clamped * 100f) + "%";
        }
    }
}
