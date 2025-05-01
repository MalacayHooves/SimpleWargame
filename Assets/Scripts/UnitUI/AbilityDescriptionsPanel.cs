using SimpleWargame.AbilitySystem;
using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.UnitUI
{
    public class AbilityDescriptionsPanel : AbilityPanel
    {
        [SerializeField] private UnitStatsUI unitStatsUI;

        protected override void Start()
        {

        }

        protected override void AbilityButtonClicked(Unit selectedUnit, Ability ability)
        {
            unitStatsUI.ShowDescriptionPanel(ability.AbilityName, ability.AbilityDescription);
        }

        public override void ShowPanel()
        {
            gameObject.SetActive(true);
        }
    }
}
