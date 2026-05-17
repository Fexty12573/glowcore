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

        // Per-tab sprites — order MUST match the category order from the bootstrapper
        // (currently: 0 = Audio, 1 = Display). Wire in the Inspector.
        [Header("Tab Sprites (parallel to categories)")]
        [SerializeField] private Sprite[] m_tabActiveSprites;
        [SerializeField] private Sprite[] m_tabInactiveSprites;

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
                m_backButton.onClick.AddListener(TryLeave);
            if (m_saveButton != null)
                m_saveButton.onClick.AddListener(OnSave);

            m_canvasGroup?.SetVisible(false);
        }

        public void Show()
        {
            // Snapshot lazily — only on first open of a Settings session. Dialog round-trips
            // call Show() again to re-display the screen, and re-snapshotting there would
            // capture the dirty values and break the Discard/Cancel paths.
            if (m_snapshot.Count == 0)
                SnapshotAll();

            var index = m_activeCategoryIndex >= 0 ? m_activeCategoryIndex : 0;
            if (m_categories != null && m_categories.Count > 0)
                ActivateCategory(index);

            m_canvasGroup?.SetVisible(true);
        }

        public void Hide() => m_canvasGroup?.SetVisible(false);

        public bool IsDirty()
        {
            foreach (var pair in m_snapshot)
            {
                object current = pair.Key.Read();
                if (!Equals(current, pair.Value))
                    return true;
            }
            return false;
        }

        public void TryLeave()
        {
            if (!IsDirty())
            {
                m_snapshot.Clear();
                m_menuManager?.Back();
                return;
            }

            var args = new UnsavedChangesDialogArgs
            {
                Message = "You have unsaved changes. Apply them?",
                OnApply = ApplyAndLeave,
                OnDiscard = DiscardAndLeave,
            };
            m_menuManager?.OpenWithArgs(MenuScreenId.UnsavedChanges, args);
        }

        private void OnDestroy()
        {
            if (m_backButton != null)
                m_backButton.onClick.RemoveListener(TryLeave);
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

                // Set the inactive sprite immediately so a freshly built row of tabs renders
                // correctly before any ActivateCategory call. ActivateCategory then promotes
                // the active one.
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    Sprite inactive = GetTabSprite(m_tabInactiveSprites, i);
                    if (inactive != null)
                        img.sprite = inactive;
                }

                btn.onClick.AddListener(() => ActivateCategory(index));
                m_tabButtons.Add(btn);
            }
        }

        private static Sprite GetTabSprite(Sprite[] sprites, int index)
        {
            if (sprites == null || index < 0 || index >= sprites.Length)
                return null;
            return sprites[index];
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

                Image img = tab.GetComponent<Image>();
                if (img != null)
                {
                    Sprite target = isActive ? GetTabSprite(m_tabActiveSprites, i) : GetTabSprite(m_tabInactiveSprites, i);
                    if (target != null)
                        img.sprite = target;

                    // Ensure full opacity now that sprites encode the active/inactive look themselves.
                    Color c = img.color;
                    c.a = 1f;
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

        private void RevertToSnapshot()
        {
            foreach (var pair in m_snapshot)
                pair.Key.Write(pair.Value);
        }

        private void ApplyAndLeave()
        {
            m_repository?.Save();
            m_snapshot.Clear();
            m_menuManager?.Back();
        }

        private void DiscardAndLeave()
        {
            RevertToSnapshot();
            m_snapshot.Clear();
            m_menuManager?.Back();
        }

        private void OnSave()
        {
            m_repository?.Save();
            m_snapshot.Clear();
            m_menuManager?.Back();
        }
    }
}
