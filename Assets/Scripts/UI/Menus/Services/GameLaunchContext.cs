using UnityEngine;

namespace GlowCore.UI.Menus
{
    public class GameLaunchContext : MonoBehaviour, IGameLaunchContext
    {
        private const string kDefaultWorldName = "Player";

        private static GameLaunchContext s_instance;

        private GameLaunchMode m_mode = GameLaunchMode.Continue;
        private string m_worldName = kDefaultWorldName;

        public static GameLaunchContext Instance
        {
            get
            {
                if (s_instance != null)
                    return s_instance;

                var go = new GameObject("[GameLaunchContext]");
                s_instance = go.AddComponent<GameLaunchContext>();
                DontDestroyOnLoad(go);
                return s_instance;
            }
        }

        public GameLaunchMode Mode => m_mode;
        public string WorldName => m_worldName;

        public void SetNewGame(string worldName)
        {
            m_mode = GameLaunchMode.NewGame;
            m_worldName = string.IsNullOrWhiteSpace(worldName) ? kDefaultWorldName : worldName;
        }

        public void SetContinue()
        {
            m_mode = GameLaunchMode.Continue;
        }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
