using SimpleWargame.MenuManagement;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.UnitUI
{
    public class UnitUILoader : MonoBehaviour
    {
        private const string UNIT_UI_SCENE_NAME = "UnitUI";

        private async void Start()
        {
            UnitUI unitUI = FindObjectOfType<UnitUI>();

            if (unitUI == null)
            {
                if (SceneLoader.Instance == null) { Debug.LogError("BattleUIInitialization Error: there is no SceneLoader"); return; }
                SceneLoader.Instance.LoadSceneAdditive(UNIT_UI_SCENE_NAME);

                while (SceneLoader.Instance.IsLoading) await Task.Yield();
                
                unitUI = FindObjectOfType<UnitUI>();
            }

            unitUI.SubscribeToManagerEvents();
        }
    }
}
