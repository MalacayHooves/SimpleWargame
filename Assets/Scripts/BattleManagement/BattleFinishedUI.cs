using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleWargame.MenuManagement;

namespace SimpleWargame.BattleManagement
{
    public class BattleFinishedUI : MonoBehaviour
    {
        public static BattleFinishedUI Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI battleFinishedText;
        [SerializeField] private Button exitToMenuButton;

        private const string MAIN_MENU_SCENE_NAME = "MainMenu";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            gameObject.SetActive(false);
        }

        private void Start()
        {
            if (exitToMenuButton == null) { Debug.LogError("BattleFinishedUI Error: there is no ExitToMenuButton"); return; }

            exitToMenuButton.onClick.AddListener(() =>
            {
                if (SceneLoader.Instance == null) { Debug.LogError("ExitToMenuButton Error: there is no SceneLoader"); return; }
                SceneLoader.Instance.LoadScene(MAIN_MENU_SCENE_NAME);
            });
        }

        public void ShowFinishBattleScreen(string finishBattleText)
        {
            gameObject.SetActive(true);

            battleFinishedText.text = finishBattleText;
        }
    }
}
