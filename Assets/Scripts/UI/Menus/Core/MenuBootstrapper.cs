using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlowCore.UI.Menus
{
    public class MenuBootstrapper : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private TitleScreen m_titleScreen;
        [SerializeField] private NewGameScreen m_newGameScreen;
        [SerializeField] private ConfirmDialogScreen m_confirmDialogScreen;
        [SerializeField] private SettingsScreen m_settingsScreen;

        [Header("Settings row prefabs")]
        [SerializeField] private SliderSettingControl m_sliderRowPrefab;
        [SerializeField] private ToggleSettingControl m_toggleRowPrefab;
        [SerializeField] private DropdownSettingControl m_dropdownRowPrefab;

        [Header("Audio")]
        [SerializeField] private AudioMixerVolumeApplier m_audioApplier;

        private MenuManager m_menuManager;

        private void Awake()
        {
            // Refuse to boot a second instance — the save file at SaveData.GetPath() is shared
            // across processes and concurrent writes would corrupt it.
#if UNITY_EDITOR
            ISingleInstanceGuard instanceGuard = new NullSingleInstanceGuard();
#else
            ISingleInstanceGuard instanceGuard = new NamedMutexSingleInstanceGuard(NamedMutexSingleInstanceGuard.kDefaultName);
#endif
            if (!ProcessGuard.TryAcquireOnce(instanceGuard))
            {
                Debug.LogWarning("[MenuBootstrapper] Another GlowCore instance is already running. Quitting.");
                Application.Quit();
                return;
            }

            // Persistent context (DontDestroyOnLoad) carries the new-game world name across the scene load.
            IGameLaunchContext launchContext = GameLaunchContext.Instance;

            // Service composition (plain C# unless engine API is required).
            ISaveDataGateway saveDataGateway = new SaveDataGateway();
            ISceneTransition sceneTransition = new SceneTransitionService();
            INewGameService newGameService = new NewGameService(saveDataGateway, launchContext, sceneTransition);

            IPlayerPrefsBackend prefsBackend = new UnityPlayerPrefsBackend();
            ISettingsRepository settingsRepository = new PlayerPrefsSettingsRepository(prefsBackend);
            IAudioBridge audioBridge = new AudioBridge();
            IDisplayService displayService = new DisplayService();

            // Mixer applier subscribes to the bridge.
            m_audioApplier?.Initialize(audioBridge);

            // Apply saved settings before any UI shows so initial values are correct.
            ApplyPersistedSettings(settingsRepository, audioBridge, displayService);

            // Menu manager + screen registry.
            var locator = new MenuScreenLocator();
            m_menuManager = new MenuManager(locator);

            if (m_titleScreen != null)
            {
                m_titleScreen.Initialize(m_menuManager, newGameService, launchContext, sceneTransition);
                locator.Register(m_titleScreen);
            }

            if (m_newGameScreen != null)
            {
                m_newGameScreen.Initialize(m_menuManager, newGameService);
                locator.Register(m_newGameScreen);
            }

            if (m_confirmDialogScreen != null)
            {
                m_confirmDialogScreen.Initialize(m_menuManager);
                locator.Register(m_confirmDialogScreen);
            }

            if (m_settingsScreen != null)
            {
                var rowFactory = BuildRowFactory();
                var categories = new List<ISettingsCategory>
                {
                    new AudioSettingsCategory(settingsRepository, audioBridge),
                    new DisplaySettingsCategory(settingsRepository, displayService),
                };
                m_settingsScreen.Initialize(m_menuManager, settingsRepository, rowFactory, categories);
                locator.Register(m_settingsScreen);
            }
        }

        private void Start()
        {
            m_menuManager?.Open(MenuScreenId.Title);
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

            displayService.SetCameraSensitivityX(repository.GetFloat(
                DisplaySettingsCategory.kKeyCameraSensitivityX,
                DisplayService.kSensitivityDefaultSlider));
            displayService.SetCameraSensitivityY(repository.GetFloat(
                DisplaySettingsCategory.kKeyCameraSensitivityY,
                DisplayService.kSensitivityDefaultSlider));

            // Note: fullscreen and resolution are intentionally left to whatever the OS gave us at boot
            // unless the user changed them — we only apply via the Apply path on Save to avoid resolution
            // flicker on every launch.
        }
    }
}
