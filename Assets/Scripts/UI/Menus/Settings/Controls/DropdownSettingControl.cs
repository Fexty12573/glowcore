using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class DropdownSettingControl : MonoBehaviour, ISettingControl
    {
        [SerializeField] private TMP_Dropdown m_dropdown;
        [SerializeField] private TextMeshProUGUI m_labelText;

        private SettingDescriptor m_descriptor;

        public void Bind(SettingDescriptor descriptor)
        {
            Unbind();

            m_descriptor = descriptor;

            if (m_labelText != null)
                m_labelText.text = descriptor.Label;

            if (m_dropdown != null)
            {
                m_dropdown.ClearOptions();
                if (descriptor.Options != null && descriptor.Options.Count > 0)
                    m_dropdown.AddOptions(new List<string>(descriptor.Options));

                var current = System.Convert.ToInt32(descriptor.Read());
                m_dropdown.SetValueWithoutNotify(Mathf.Clamp(current, 0, Mathf.Max(0, m_dropdown.options.Count - 1)));
                m_dropdown.RefreshShownValue();
                m_dropdown.onValueChanged.AddListener(OnDropdownChanged);
            }
        }

        public void Unbind()
        {
            if (m_dropdown != null)
                m_dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
            m_descriptor = null;
        }

        private void OnDestroy() => Unbind();

        private void OnDropdownChanged(int value)
        {
            m_descriptor?.Write(value);
        }
    }
}
