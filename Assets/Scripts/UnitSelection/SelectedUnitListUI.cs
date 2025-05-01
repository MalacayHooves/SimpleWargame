using SimpleWargame.MenuManagement;
using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SimpleWargame.UnitSelection
{
    public class SelectedUnitListUI : UnitListUI
    {
        [SerializeField] private TextMeshProUGUI totalPowerNumberText;

        protected override void Start()
        {
            base.Start();

            if (UnitSelectionUI.Instance == null) { Debug.LogError("SelectedUnitListUI Error: there is no UnitSelectionUI"); return; }
            UnitSelectionUI.Instance.SetLoadBattleMapButtonEnabled(units.Count > 0);

            totalPowerNumberText.text = "0";
        }

        public override void AddUnitToList(UnitUIElement unitUIElement)
        {
            base.AddUnitToList(unitUIElement);

            if (UnitSelectionUI.Instance == null) { Debug.LogError("SelectedUnitListUI Error: there is no UnitSelectionUI"); return; }
            UnitSelectionUI.Instance.SetLoadBattleMapButtonEnabled(units.Count > 0);

            if (PersistentDataStorage.Instance == null) { Debug.LogError("SelectedUnitListUI Error: there is no PersistentDataStorage"); return; }
            PersistentDataStorage.Instance.SetUnits(units);

            int totalCost = 0;
            foreach (Unit unit in units)
            {
                totalCost += unit.UnitStats.UnitCost;
            }

            totalPowerNumberText.text = totalCost.ToString();
        }

        public override void RemoveUnitFromList(UnitUIElement unitUIElement)
        {
            base.RemoveUnitFromList(unitUIElement);

            int totalCost = 0;
            foreach (Unit unit in units)
            {
                totalCost += unit.UnitStats.UnitCost;
            }

            totalPowerNumberText.text = totalCost.ToString();
        }
    }
}
