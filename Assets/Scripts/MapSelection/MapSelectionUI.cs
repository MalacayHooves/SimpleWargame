using SimpleWargame.MenuManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.MapSelection
{
    public class MapSelectionUI : MonoBehaviour
    {
        private const string MAIN_MENU_SCENE_NAME = "MainMenu";
        private const string UNIT_SELECTION_SCENE_NAME = "UnitSelectionScene";

        [Header("Links to Objects")]
        [SerializeField] private Button selectMapButtonPrefab;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button goToUnitSelectionButton;
        [SerializeField] private TextMeshProUGUI selectedMapText;
        [SerializeField] private Transform mapNamesButtonsPanel;
        [SerializeField] private Image mapPreview;
        [SerializeField] private TextMeshProUGUI enemyPowerNumberText;

        [Header("Variables")]
        [SerializeField] private List<SceneWithPreview> sceneNamesWithPreviews;

        [Serializable]
        public struct SceneWithPreview
        {
            public string ActualSceneName;
            public string DisplayedName;
            public Sprite PreviewSprite;
            public int EnemyPower;
        }

        private void Start()
        {
            backToMenuButton.onClick.AddListener(() =>
            {
                if (SceneLoader.Instance == null) { Debug.LogError("MapSelectionUI Error: there is no SceneLoader"); return; }
                SceneLoader.Instance.LoadScene(MAIN_MENU_SCENE_NAME);
            });

            goToUnitSelectionButton.onClick.AddListener(() =>
            {
                if (SceneLoader.Instance == null) { Debug.LogError("MapSelectionUI Error: there is no SceneLoader"); return; }
                SceneLoader.Instance.LoadScene(UNIT_SELECTION_SCENE_NAME);
            });

            if (sceneNamesWithPreviews == null) { Debug.LogError("MapSelectionUI Error: there is no added maps"); return; }
            bool isSceneWithPreviewDisplayed = false;
            foreach (SceneWithPreview scene in sceneNamesWithPreviews)
            {
                Button button = Instantiate(selectMapButtonPrefab, mapNamesButtonsPanel);
                button.GetComponentInChildren<TextMeshProUGUI>().text = scene.DisplayedName;

                button.onClick.AddListener(() =>
                {
                    SetMapToLoadAndDisplay(scene);
                });

                if (!isSceneWithPreviewDisplayed)
                {
                    isSceneWithPreviewDisplayed = true;

                    SetMapToLoadAndDisplay(scene);
                }
            }
        }

        private void SetMapToLoadAndDisplay(SceneWithPreview scene)
        {
            selectedMapText.text = scene.DisplayedName;
            mapPreview.sprite = scene.PreviewSprite;
            if (PersistentDataStorage.Instance == null) { Debug.LogError("MapSelectionUI Error: there is no PersistentDataStorage"); return; }
            PersistentDataStorage.Instance.MapToLoadName = scene.ActualSceneName;
            enemyPowerNumberText.text = scene.EnemyPower.ToString();
        }
    }
}
