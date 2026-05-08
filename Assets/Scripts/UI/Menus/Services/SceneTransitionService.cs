using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlowCore.UI.Menus
{
    public class SceneTransitionService : ISceneTransition
    {
        public const string kTitleSceneName = "TitleScene";
        public const string kMainWorldSceneName = "MainWorldScene";

        public void LoadTitleScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(kTitleSceneName);
        }

        public void LoadMainWorldScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(kMainWorldSceneName);
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            Debug.Log("[SceneTransitionService] QuitApplication called (editor: stopping play mode)");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
