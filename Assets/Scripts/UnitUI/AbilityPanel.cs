using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Units;
using SimpleWargame.AbilitySystem;
using System.Linq;
using UnityEngine.UI;
using SimpleWargame.BattleManagement;
using System;

namespace SimpleWargame.UnitUI
{
    /// <summary>
    /// This class handles UI side of ability system: gets selected unit and creates button to fire its abilities
    /// </summary>
    public class AbilityPanel : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField] private bool showWhenUnitSelected;

        [Header("Links to Objects")]
        [SerializeField] private RectTransform abilityButtonPrefab;

        private Dictionary<RectTransform, Button> abilityButtons;

        ICanSendSelectedUnit manager;

        private void Awake()
        {
            abilityButtons = new Dictionary<RectTransform, Button>();
        }

        protected virtual void Start()
        {
            HidePanel();
        }

        public void SubscribeToManagerEvents(ICanSendSelectedUnit manager)
        {
            if (manager == null) { Debug.LogError("AbilityPanel Error: there is no Manager able to send unit stats"); return; }

            this.manager = manager;

            this.manager.OnSelectUnit += Manager_OnSelectUnit;
        }

        public void UnsubscribeFromManagerEvents(ICanSendSelectedUnit manager)
        {
            if (manager == null) { Debug.LogError("AbilityPanel Error: there is no Manager able to send unit stats"); return; }

            manager.OnSelectUnit -= Manager_OnSelectUnit;
        }

        public virtual void ShowPanel()
        {
            if (BattleManager.Instance == null) return;
            if (!BattleManager.Instance.IsSpawnFinished) return;
            if (BattleManager.Instance.ActivePlayer.IsUnderAIControl) return;
            gameObject.SetActive(true);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }

        private void Manager_OnSelectUnit(object sender, EventArgs e)
        {
            foreach (Button button in abilityButtons.Values)
            {
                button.onClick.RemoveAllListeners();
            }
            foreach (RectTransform abilityButton in abilityButtons.Keys)
            {
                abilityButton.gameObject.SetActive(false);
            }
            if (manager.SelectedUnit != null)
            {
                for (int i = 0; i < manager.SelectedUnit.Abilities.Count; i++)
                {
                    RectTransform prefab;
                    Button button;
                    if (abilityButtons.Keys.ElementAtOrDefault(i) == null)
                    {
                        prefab = Instantiate(abilityButtonPrefab, this.transform);
                        button = prefab.GetComponentInChildren<Button>();
                        abilityButtons.Add(prefab, button);
                    }
                    else
                    {
                        prefab = abilityButtons.Keys.ElementAt(i);
                        prefab.gameObject.SetActive(true);
                        button = prefab.GetComponentInChildren<Button>();
                    }
                    button.image.sprite = manager.SelectedUnit.Abilities[i].AbilityIcon;
                    int index = i;
                    button.onClick.AddListener(delegate
                    {
                        AbilityButtonClicked(manager.SelectedUnit,
                        manager.SelectedUnit.Abilities[index]);
                    });
                }

                if (showWhenUnitSelected) ShowPanel();
            }
            else HidePanel();
        }

        protected virtual void AbilityButtonClicked(Unit selectedUnit, Ability ability)
        {
            if (BattleManager.Instance == null) return;
            if (selectedUnit.Player == BattleManager.Instance.ActivePlayer && !selectedUnit.IsTurnFinished) ability.Activate(selectedUnit);
        }
    }
}
