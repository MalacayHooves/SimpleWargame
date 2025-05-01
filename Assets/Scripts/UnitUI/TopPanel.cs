using SimpleWargame.Map;
using SimpleWargame.BattleManagement;
using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace SimpleWargame.UnitUI
{
    /// <summary>
    /// This class handles top panel of UI. It shows selected unit name and has button to open this unit stats
    /// </summary>
    public class TopPanel : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private UnitStatsUI unitStatsPanel;
        [SerializeField] private AbilityPanel abilityPanel;
        [SerializeField] private AbilityDescriptionsPanel abilityDescriptionsPanel;
        [SerializeField] protected Button showUnitStatsButton;
        [SerializeField] protected Button closeUnitStatsButton;

        ICanSendSelectedUnit manager;

        private void Start()
        {
            showUnitStatsButton.onClick.AddListener(ToggleUnitStatsPanel);

            closeUnitStatsButton.onClick.AddListener(() => { HidePanels(); });

            HidePanels();
        }

        public void SubscribeToManagerEvents(ICanSendSelectedUnit manager)
        {
            if (manager == null) { Debug.LogError("TopPanel: there is no Manager able to send unit stats"); return; }
            
            this.manager = manager;

            this.manager.OnSelectUnit += Manager_OnSelectUnit;
        }

        public void UnsubscribeFromManagerEvents(ICanSendSelectedUnit manager)
        {
            if (manager == null) { Debug.LogError("TopPanel: there is no Manager able to send unit stats"); return; }

            manager.OnSelectUnit -= Manager_OnSelectUnit;
        }

       public void ShowPanels()
        {
            if (manager.SelectedUnit == null)
            {
                unitNameText.text = " ";
                return;
            }

            unitNameText.text = manager.SelectedUnit.UnitName;

            unitStatsPanel.ShowPanel(manager.SelectedUnit);
            abilityPanel.HidePanel();
            abilityDescriptionsPanel.ShowPanel();
        }

        public void HidePanels()
        {
            unitNameText.text = " ";
            unitStatsPanel.gameObject.SetActive(false);
            abilityPanel.ShowPanel();
            abilityDescriptionsPanel.HidePanel();
        }

        private void ToggleUnitStatsPanel()
        {
            if (unitStatsPanel.gameObject.activeSelf) HidePanels();
            else ShowPanels();
        }

        private void Manager_OnSelectUnit(object sender, EventArgs e)
        {
            if (manager.SelectedUnit == null)
            {
                unitNameText.text = " ";
                return;
            }
            unitNameText.text = manager.SelectedUnit.UnitName;
        }
    }
}
