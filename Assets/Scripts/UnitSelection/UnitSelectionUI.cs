using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.UnitUI;
using SimpleWargame.Units;
using UnityEngine.UI;
using SimpleWargame.MenuManagement;

namespace SimpleWargame.UnitSelection
{
    public class UnitSelectionUI : MonoBehaviour, ICanSendSelectedUnit
    {
        public static UnitSelectionUI Instance;

        private const string MAP_SELECTION_SCENE_NAME = "MapSelectionScene";

        [Header("Links to Objects")]
        [SerializeField] private Button loadMapSelectionButton;
        [SerializeField] private Button loadBattleMapButton;

        public Unit SelectedUnit => selectedUnit;
        public UnitUIElement SelectedUnitUIElement => selectedUnitUIElement;

        public event EventHandler OnSelectUnit;

        private Unit selectedUnit;
        private UnitUIElement selectedUnitUIElement;

        private void Start()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            loadMapSelectionButton.onClick.AddListener(() => {
                if (SceneLoader.Instance == null) { Debug.LogError("ExitToMenuButton Error: there is no SceneLoader"); return; }
                if (PersistentDataStorage.Instance == null) { Debug.LogError("UnitSelectionUI Error: there is no PersistentDataStorage"); return; }
                PersistentDataStorage.Instance.ClearUnits();
                SceneLoader.Instance.LoadScene(MAP_SELECTION_SCENE_NAME);
            });
            loadBattleMapButton.onClick.AddListener(() => {
                if (SceneLoader.Instance == null) { Debug.LogError("ExitToMenuButton Error: there is no SceneLoader"); return; }
                if (PersistentDataStorage.Instance == null) { Debug.LogError("UnitSelectionUI Error: there is no PersistentDataStorage"); return; }
                SceneLoader.Instance.LoadScene(PersistentDataStorage.Instance.MapToLoadName);
            });

            OnSelectUnit?.Invoke(this, EventArgs.Empty);
        }

        private void OnDisable()
        {
            UnsubscribeUIFromManagerEvents();
        }

        public void UnsubscribeUIFromManagerEvents()
        {
            UnitUI.UnitUI unitUI = MonoBehaviour.FindAnyObjectByType<UnitUI.UnitUI>();
            if (unitUI != null) unitUI.UnsubscribeFromManagerEvents();
        }

        public void SelectUnit(UnitUIElement unitUIElement)
        {
            selectedUnitUIElement = unitUIElement;
            selectedUnit = unitUIElement.Unit;
            OnSelectUnit?.Invoke(this, EventArgs.Empty);
        }

        public void SetLoadBattleMapButtonEnabled(bool isEnabled)
        {
            loadBattleMapButton.interactable = isEnabled;

        }
    }
}
