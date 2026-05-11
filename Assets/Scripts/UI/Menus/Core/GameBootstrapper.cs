using System.Collections.Generic;
using GlowCore.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlowCore.UI.Menus
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private PauseScreen m_pauseScreen;
        [SerializeField] private SettingsScreen m_settingsScreen;
        [SerializeField] private ConfirmDialogScreen m_confirmDialogScreen;

        [Header("HUD")]
        [SerializeField] private PauseButton m_pauseButton;

        [Header("Settings row prefabs")]
        [SerializeField] private SliderSettingControl m_sliderRowPrefab;
        [SerializeField] private ToggleSettingControl m_toggleRowPrefab;
        [SerializeField] private DropdownSettingControl m_dropdownRowPrefab;

        [Header("Audio")]
        [SerializeField] private AudioMixerVolumeApplier m_audioApplier;

        [Header("Scene refs")]
        [SerializeField] private WorldGrid m_worldGrid;
        [SerializeField] private PlayerCamera m_playerCamera;
        [SerializeField] private PlayerInventory m_playerInventory;

        [Header("Input")]
        [SerializeField] private InputActionAsset m_inputActions;
        [SerializeField] private string m_pauseActionName = "PauseEscape";

        private MenuManager m_menuManager;
        private GameStateController m_gameState;
        private PauseInputHandler m_inputHandler;

        private void Awake()
        {
            // Refuse to boot a second instance — guards the save file when MainWorldScene is the
            // first scene loaded (e.g. dev play-mode iteration that skips TitleScene).
#if UNITY_EDITOR
            ISingleInstanceGuard instanceGuard = new NullSingleInstanceGuard();
#else
            ISingleInstanceGuard instanceGuard = new NamedMutexSingleInstanceGuard(NamedMutexSingleInstanceGuard.kDefaultName);
#endif
            if (!ProcessGuard.TryAcquireOnce(instanceGuard))
            {
                Debug.LogWarning("[GameBootstrapper] Another GlowCore instance is already running. Quitting.");
                Application.Quit();
                return;
            }

            IGameLaunchContext launchContext = GameLaunchContext.Instance;

            ISceneTransition sceneTransition = new SceneTransitionService();
            ISaveService saveService = new SaveService();

            IPlayerPrefsBackend prefsBackend = new UnityPlayerPrefsBackend();
            ISettingsRepository settingsRepository = new PlayerPrefsSettingsRepository(prefsBackend);
            IAudioBridge audioBridge = new AudioBridge();
            IDisplayService displayService = new DisplayService();

            m_audioApplier?.Initialize(audioBridge);
            ApplyPersistedSettings(settingsRepository, audioBridge, displayService);

            m_gameState = new GameStateController(saveService, sceneTransition);

            var locator = new MenuScreenLocator();
            m_menuManager = new MenuManager(locator);

            if (m_pauseScreen != null)
            {
                m_pauseScreen.Initialize(m_menuManager, m_gameState);
                locator.Register(m_pauseScreen);
            }

            if (m_confirmDialogScreen != null)
            {
                m_confirmDialogScreen.Initialize(m_menuManager);
                locator.Register(m_confirmDialogScreen);
            }

            if (m_settingsScreen != null)
            {
                SettingsRowFactory rowFactory = BuildRowFactory();
                var categories = new List<ISettingsCategory>
                {
                    new AudioSettingsCategory(settingsRepository, audioBridge),
                    new DisplaySettingsCategory(settingsRepository, displayService),
                };
                m_settingsScreen.Initialize(m_menuManager, settingsRepository, rowFactory, categories);
                locator.Register(m_settingsScreen);
            }

            m_pauseButton?.Initialize(m_gameState, m_menuManager);

            // Esc dispatch — registered consumers run highest-priority first; the first one
            // that consumes the keypress wins. Adding a new screen that should swallow Esc
            // means a new IEscapeConsumer + register here, no edits to PauseInputHandler.
            var router = new EscapeRouter();
            PlayerInventory inventory = m_playerInventory != null ? m_playerInventory : FindFirstObjectByType<PlayerInventory>();
            if (inventory != null)
                router.Register(new InventoryEscapeConsumer(inventory));
            router.Register(new PauseEscapeConsumer(m_menuManager, m_gameState));

            InputAction pauseAction = m_inputActions != null ? m_inputActions.FindAction(m_pauseActionName) : null;
            if (pauseAction == null)
                Debug.LogWarning($"[GameBootstrapper] InputAction '{m_pauseActionName}' not found on the configured InputActionAsset; Esc will not trigger pause.");

            m_inputHandler = gameObject.AddComponent<PauseInputHandler>();
            m_inputHandler.Initialize(router, pauseAction);

            m_worldGrid?.SetLaunchContext(launchContext);
            m_playerCamera?.Initialize(displayService);
        }

        private SettingsRowFactory BuildRowFactory()
        {
            var factory = new SettingsRowFactory();
            if (m_sliderRowPrefab != null)
                factory.Register(SettingControlKind.Slider, parent => Instantiate(m_sliderRowPrefab, parent));
            if (m_toggleRowPrefab != null)
                factory.Register(SettingControlKind.Toggle, parent => Instantiate(m_toggleRowPrefab, parent));
            if (m_dropdownRowPrefab != null)
                factory.Register(SettingControlKind.Dropdown, parent => Instantiate(m_dropdownRowPrefab, parent));
            return factory;
        }

        private static void ApplyPersistedSettings(
            ISettingsRepository repository,
            IAudioBridge audioBridge,
            IDisplayService displayService)
        {
            audioBridge.RaiseMaster(repository.GetFloat(AudioSettingsCategory.kKeyMaster, AudioSettingsCategory.kDefaultMaster));
            audioBridge.RaiseMusic(repository.GetFloat(AudioSettingsCategory.kKeyMusic, AudioSettingsCategory.kDefaultMusic));
            audioBridge.RaiseSfx(repository.GetFloat(AudioSettingsCategory.kKeySfx, AudioSettingsCategory.kDefaultSfx));

            displayService.SetMouseSensitivity(repository.GetFloat(
                DisplaySettingsCategory.kKeyMouseSensitivity,
                DisplayService.kSensitivityDefaultSlider));
        }
    }
}
