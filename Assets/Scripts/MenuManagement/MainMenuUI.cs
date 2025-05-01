using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.MenuManagement
{
    /// <summary>
    /// This class handles main menu UI
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button loadUnitSelectionSceneButton;
        [SerializeField] private Button exitGameButton;

        private const string MAP_SELECTION_SCENE_NAME = "MapSelectionScene";

        private void Start()
        {
            loadUnitSelectionSceneButton.onClick.AddListener(() =>
            {
                if (SceneLoader.Instance == null) { Debug.LogError("MainMenuUI Error: there is no SceneLoader"); return; }
                SceneLoader.Instance.LoadScene(MAP_SELECTION_SCENE_NAME);
            });

            exitGameButton.onClick.AddListener(() => { Application.Quit(); });
        }
    }
}
