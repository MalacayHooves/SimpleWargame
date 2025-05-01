using SimpleWargame.EffectSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.UnitUI
{
    public class EffectIcon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private Image iconImage;

        private UnitStatsUI unitStatsUI;
        private Effect effect;

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => { unitStatsUI.ShowDescriptionPanel(effect.EffectName, effect.EffectDescription); });
        }

        public void SetIcon(UnitStatsUI unitStatsUI, Effect effect, string cooldownString)
        {
            iconImage.sprite = effect.EffectSprite;
            cooldownText.text = cooldownString;
            this.unitStatsUI = unitStatsUI;
            this.effect = effect;
        }
    }
}
