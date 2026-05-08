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
            SceneTransitionOverlay.Instance.LoadScene(kTitleSceneName, "Returning home...");
        }

        public void LoadMainWorldScene()
        {
            Time.timeScale = 1f;
            SceneTransitionOverlay.Instance.LoadScene(kMainWorldSceneName, "Awakening...");
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
