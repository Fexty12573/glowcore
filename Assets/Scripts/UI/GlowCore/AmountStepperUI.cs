using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Upgrade
{
    public class AmountStepperUI : MonoBehaviour
    {
        [SerializeField] private Button m_minusButton;
        [SerializeField] private Button m_plusButton;
        [SerializeField] private TextMeshProUGUI m_valueLabel;

        private int m_value;
        private int m_min;
        private int m_max;

        public int Value => m_value;
        public int Min => m_min;
        public int Max => m_max;

        public event Action<int> OnValueChanged;

        public void Bind(int min, int max, int initialValue)
        {
            m_min = min;
            m_max = Mathf.Max(min, max);
            SetValue(Mathf.Clamp(initialValue, m_min, m_max), fireEvent: false);
        }

        public void SetBounds(int min, int max)
        {
            m_min = min;
            m_max = Mathf.Max(min, max);
            SetValue(Mathf.Clamp(m_value, m_min, m_max), fireEvent: true);
        }

        public void SetValue(int value, bool fireEvent = true)
        {
            var clamped = Mathf.Clamp(value, m_min, m_max);
            if (clamped == m_value)
            {
                UpdateVisuals();
                return;
            }

            m_value = clamped;
            UpdateVisuals();
            if (fireEvent)
                OnValueChanged?.Invoke(m_value);
        }

        private void Awake()
        {
            if (m_minusButton != null)
                m_minusButton.onClick.AddListener(OnMinusClicked);
            if (m_plusButton != null)
                m_plusButton.onClick.AddListener(OnPlusClicked);
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            if (m_minusButton != null)
                m_minusButton.onClick.RemoveListener(OnMinusClicked);
            if (m_plusButton != null)
                m_plusButton.onClick.RemoveListener(OnPlusClicked);
        }

        private void OnMinusClicked() => SetValue(m_value - 1);

        private void OnPlusClicked() => SetValue(m_value + 1);

        private void UpdateVisuals()
        {
            if (m_valueLabel != null)
                m_valueLabel.text = m_value.ToString();
            if (m_minusButton != null)
                m_minusButton.interactable = m_value > m_min;
            if (m_plusButton != null)
                m_plusButton.interactable = m_value < m_max;
        }
    }
}
