using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.Units
{
    public class UnitHealthbar : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private Image barImage;
        [SerializeField] private TextMeshProUGUI unitCountText;

        private Unit unit;

        public void SetUnit(Unit newUnit)
        {
            unit = newUnit;
            unit.OnHealthChanged += Unit_OnAttackFinished;
            RefreshHeathbar();
        }

        private void Unit_OnAttackFinished(object sender, EventArgs e)
        {
            RefreshHeathbar();
        }

        private void RefreshHeathbar()
        {
            unitCountText.text = unit.UnitStats.UnitCount + "/" + unit.BaseUnitStats.UnitCount;
            barImage.fillAmount = (float)unit.UnitStats.Health / unit.BaseUnitStats.Health;
        }
    }
}
