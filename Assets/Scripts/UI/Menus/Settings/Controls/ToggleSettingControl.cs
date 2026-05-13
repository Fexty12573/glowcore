using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class ToggleSettingControl : MonoBehaviour, ISettingControl
    {
        [SerializeField] private Toggle m_toggle;
        [SerializeField] private TextMeshProUGUI m_labelText;

        private SettingDescriptor m_descriptor;

        public void Bind(SettingDescriptor descriptor)
        {
            Unbind();

            m_descriptor = descriptor;

            if (m_labelText != null)
                m_labelText.text = descriptor.Label;

            if (m_toggle != null)
            {
                var current = System.Convert.ToBoolean(descriptor.Read());
                m_toggle.SetIsOnWithoutNotify(current);
                m_toggle.onValueChanged.AddListener(OnToggleChanged);
            }
        }

        public void Unbind()
        {
            if (m_toggle != null)
                m_toggle.onValueChanged.RemoveListener(OnToggleChanged);
            m_descriptor = null;
        }

        private void OnDestroy() => Unbind();

        private void OnToggleChanged(bool value)
        {
            m_descriptor?.Write(value);
        }
    }
}
