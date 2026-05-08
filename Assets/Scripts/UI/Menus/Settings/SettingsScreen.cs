using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class SettingsScreen : MonoBehaviour, IMenuScreen
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Transform m_tabButtonContainer;
        [SerializeField] private Button m_tabButtonPrefab;
        [SerializeField] private Transform m_contentContainer;
        [SerializeField] private Button m_backButton;
        [SerializeField] private Button m_saveButton;

        private IMenuManager m_menuManager;
        private ISettingsRepository m_repository;
        private SettingsRowFactory m_rowFactory;
        private IReadOnlyList<ISettingsCategory> m_categories;

        private readonly Dictionary<SettingDescriptor, object> m_snapshot = new();
        private readonly List<ISettingControl> m_activeControls = new();
        private readonly List<Button> m_tabButtons = new();
        private int m_activeCategoryIndex = -1;

        public MenuScreenId Id => MenuScreenId.Settings;
        public bool BlocksGameplay => true;

        public void Initialize(
            IMenuManager menuManager,
            ISettingsRepository repository,
            SettingsRowFactory rowFactory,
            IReadOnlyList<ISettingsCategory> categories)
        {
            m_menuManager = menuManager;
            m_repository = repository;
            m_rowFactory = rowFactory;
            m_categories = categories;

            BuildTabButtons();

            if (m_backButton != null)
                m_backButton.onClick.AddListener(OnBack);
            if (m_saveButton != null)
                m_saveButton.onClick.AddListener(OnSave);

            m_canvasGroup?.SetVisible(false);
        }

        public void Show()
        {
            SnapshotAll();

            var index = m_activeCategoryIndex >= 0 ? m_activeCategoryIndex : 0;
            if (m_categories != null && m_categories.Count > 0)
                ActivateCategory(index);

            m_canvasGroup?.SetVisible(true);
        }

        public void Hide() => m_canvasGroup?.SetVisible(false);

        private void OnDestroy()
        {
            if (m_backButton != null)
                m_backButton.onClick.RemoveListener(OnBack);
            if (m_saveButton != null)
                m_saveButton.onClick.RemoveListener(OnSave);

            ClearContent();
        }

        private void BuildTabButtons()
        {
            if (m_tabButtonPrefab == null || m_tabButtonContainer == null || m_categories == null)
                return;

            for (var i = 0; i < m_categories.Count; i++)
            {
                var index = i;
                Button btn = Instantiate(m_tabButtonPrefab, m_tabButtonContainer);
                btn.gameObject.SetActive(true);
                var label = btn.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
                if (label != null)
                    label.text = m_categories[i].DisplayName;
                btn.onClick.AddListener(() => ActivateCategory(index));
                m_tabButtons.Add(btn);
            }
        }

        private void ActivateCategory(int index)
        {
            if (m_categories == null || index < 0 || index >= m_categories.Count)
                return;

            m_activeCategoryIndex = index;
            ClearContent();

            ISettingsCategory category = m_categories[index];
            foreach (SettingDescriptor descriptor in category.Descriptors)
            {
                ISettingControl control = m_rowFactory?.Create(descriptor, m_contentContainer);
                if (control != null)
                    m_activeControls.Add(control);
            }

            for (var i = 0; i < m_tabButtons.Count; i++)
            {
                var isActive = i == index;
                Button tab = m_tabButtons[i];

                var label = tab.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
                if (label != null)
                    label.color = isActive ? (Color)UIColors.Accent : (Color)UIColors.WhiteDim;

                Image img = tab.GetComponent<Image>();
                if (img != null)
                {
                    Color c = img.color;
                    c.a = isActive ? 1f : 0.35f;
                    img.color = c;
                }
            }
        }

        private void ClearContent()
        {
            foreach (ISettingControl control in m_activeControls)
            {
                control.Unbind();
                if (control is MonoBehaviour mb && mb != null)
                    Destroy(mb.gameObject);
            }
            m_activeControls.Clear();
        }

        private void SnapshotAll()
        {
            m_snapshot.Clear();
            if (m_categories == null)
                return;

            foreach (ISettingsCategory category in m_categories)
            {
                foreach (SettingDescriptor descriptor in category.Descriptors)
                    m_snapshot[descriptor] = descriptor.Read();
            }
        }

        private void OnBack()
        {
            foreach (var pair in m_snapshot)
                pair.Key.Write(pair.Value);

            m_menuManager?.Back();
        }

        private void OnSave()
        {
            m_repository?.Save();
            m_menuManager?.Back();
        }
    }
}
