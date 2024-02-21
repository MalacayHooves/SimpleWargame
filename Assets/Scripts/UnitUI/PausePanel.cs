using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleWargame.MenuManagement;

namespace SimpleWargame.UnitUI
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button exitToMenuButton;

        private const string MAIN_MENU_SCENE_NAME = "MainMenu";

        private void Start()
        {
            if (resumeButton == null) { Debug.LogError("PausePanel Error: there is no ResumeButton"); return; }
            if (exitToMenuButton == null) { Debug.LogError("PausePanel Error: there is no ExitToMenuButton"); return; }

            resumeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });

            exitToMenuButton.onClick.AddListener(() =>
            {
                if (SceneLoader.Instance == null) { Debug.LogError("ExitToMenuButton Error: there is no SceneLoader"); return; }
                gameObject.SetActive(false);
                SceneLoader.Instance.LoadScene(MAIN_MENU_SCENE_NAME);
            });
        }
    }
}
