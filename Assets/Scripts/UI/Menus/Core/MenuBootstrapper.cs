using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class MenuBootstrapper : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private TitleScreen m_titleScreen;
        [SerializeField] private NewGameScreen m_newGameScreen;
        [SerializeField] private ConfirmDialogScreen m_confirmDialogScreen;

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

            // Mixer applier subscribes to the bridge.
            m_audioApplier?.Initialize(audioBridge);

            // Apply saved audio levels before the title shows so menu audio uses the right volume.
            // Display/camera settings are applied later by GameBootstrapper since they only matter in-game.
            ApplyPersistedAudio(settingsRepository, audioBridge);

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
        }

        private void Start()
        {
            m_menuManager?.Open(MenuScreenId.Title);
        }

        private static void ApplyPersistedAudio(ISettingsRepository repository, IAudioBridge audioBridge)
        {
            audioBridge.RaiseMaster(repository.GetFloat(AudioSettingsCategory.kKeyMaster, AudioSettingsCategory.kDefaultMaster));
            audioBridge.RaiseMusic(repository.GetFloat(AudioSettingsCategory.kKeyMusic, AudioSettingsCategory.kDefaultMusic));
            audioBridge.RaiseSfx(repository.GetFloat(AudioSettingsCategory.kKeySfx, AudioSettingsCategory.kDefaultSfx));
        }
    }
}
