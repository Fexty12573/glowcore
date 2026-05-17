using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class SliderSettingControl : MonoBehaviour, ISettingControl
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private TextMeshProUGUI m_labelText;
        [SerializeField] private TextMeshProUGUI m_valueText;

        private SettingDescriptor m_descriptor;

        public void Bind(SettingDescriptor descriptor)
        {
            Unbind();

            m_descriptor = descriptor;

            if (m_labelText != null)
                m_labelText.text = descriptor.Label;

            if (m_slider != null)
            {
                m_slider.minValue = descriptor.MinValue;
                m_slider.maxValue = descriptor.MaxValue;
                var current = System.Convert.ToSingle(descriptor.Read());
                m_slider.SetValueWithoutNotify(current);
                UpdateValueText(current);
                m_slider.onValueChanged.AddListener(OnSliderChanged);
            }
        }

        public void Unbind()
        {
            if (m_slider != null)
                m_slider.onValueChanged.RemoveListener(OnSliderChanged);
            m_descriptor = null;
        }

        private void OnDestroy() => Unbind();

        private void OnSliderChanged(float value)
        {
            m_descriptor?.Write(value);
            UpdateValueText(value);
        }

        private void UpdateValueText(float value)
        {
            if (m_valueText == null)
                return;
            m_valueText.text = $"{Mathf.RoundToInt(Mathf.InverseLerp(m_slider.minValue, m_slider.maxValue, value) * 100f)}%";
        }
    }
}
