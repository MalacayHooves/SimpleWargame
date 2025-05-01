using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Units;
using UnityEngine.UI;
using TMPro;
//using SimpleWargame.EffectSystem;
using System.Linq;
using System;

namespace SimpleWargame.UnitUI
{
    /// <summary>
    /// this class handles UI panel which shows info about unit: its descrtiption, stats, effects
    /// </summary>
    public class UnitStatsUI : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private RectTransform movementPenaltyUIPrefab;
        [SerializeField] private RectTransform movementPenaltiesUI;
        [SerializeField] private RectTransform effectIconPrefab;
        [SerializeField] private RectTransform effectsRectTransform;
        [SerializeField] private Image healthbarImage;
        [SerializeField] private TextMeshProUGUI unitDescription;
        [SerializeField] private TextMeshProUGUI healthbarText;
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private TextMeshProUGUI rangeText;
        [SerializeField] private DescriptionPanel descriptionPanel;
        [SerializeField] private Button healthbarButton;
        [SerializeField] private Button unitCountButton;
        [SerializeField] private Button speedButton;
        [SerializeField] private Button damageButton;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defenseButton;
        [SerializeField] private Button rangeButton;
        [SerializeField] private Button movementPenaliesButton;

        [Header("Variables")]
        [SerializeField] private float effectIconWidth = 200f;
        [SerializeField] private float movementPenaltyUIWidth = 150f;

        [SerializeField] private Descriptions descriptions;
        [Serializable] private class Descriptions
        {
            //health
            [SerializeField] private string healthName = "Health";
            public string HealthName => healthName;
            [SerializeField, TextArea(3, 10)] private string healthDescription = 
                "Health of the \"top\" entity in the unit, Heroes of Might and Magic-style.";
            public string HealthDescription => healthDescription;

            //unit count
            [SerializeField] private string unitCountName = "Unit count";
            public string UnitCountName => unitCountName;
            [SerializeField, TextArea(3, 10)] private string unitCountDescription =
                "Number of entities in the unit. Each entity has it's own health and separately deals damage";
            public string UnitCountDescription => unitCountDescription;

            //speed
            [SerializeField] private string speedName = "Speed";
            public string SpeedName => speedName;
            [SerializeField, TextArea(3, 10)] private string speedDescription = "How many tiles unit can traverse in one turn";
            public string SpeedDescription => speedDescription;

            //damage
            [SerializeField] private string damageName = "Damage";
            public string DamageName => damageName;
            [SerializeField, TextArea(3, 10)] private string damageDescription = "How many damage can inflict one entity of the unit";
            public string DamageDescription => damageDescription;

            //attack
            [SerializeField] private string attackName = "Attack";
            public string AttackName => attackName;
            [SerializeField, TextArea(3, 10)] private string attackDescription = "Chance to hit target";
            public string AttackDescription => attackDescription;

            //defense
            [SerializeField] private string defenseName = "Defense";
            public string DefenseName => defenseName;
            [SerializeField, TextArea(3, 10)] private string defenseDescription = "Chance to avoid damage";
            public string DefenseDescription => defenseDescription;

            //range
            [SerializeField] private string rangeName = "Range";
            public string RangeName => rangeName;
            [SerializeField, TextArea(3, 10)] private string rangeDescription = "Distance of attack";
            public string RangeDescription => rangeDescription;

            //movement penalties
            [SerializeField] private string movementPenaliesName = "Movement penalties";
            public string MovementPenaliesName => movementPenaliesName;
            [SerializeField, TextArea(3, 10)] private string movementPenaliesDescription =
                "Shows types of tiles unit can't cross or penalty for movement through";
            public string MovementPenaliesDescription => movementPenaliesDescription;
        }

        private List<RectTransform> icons = new List<RectTransform>();
        private List<RectTransform> movementPenaltyUIList = new List<RectTransform>();

        private float awayFromOutlineOffset = 5f;

        private void Start()
        {
            RectTransform movementPenaltyUI = Instantiate(movementPenaltyUIPrefab, movementPenaltiesUI);
            movementPenaltyUIList.Add(movementPenaltyUI);
            movementPenaltyUI.gameObject.SetActive(false);

            RectTransform effectIcon = Instantiate(effectIconPrefab, effectsRectTransform);
            icons.Add(effectIcon);
            effectIcon.gameObject.SetActive(false);

            healthbarButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.HealthName, descriptions.HealthDescription); });
            unitCountButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.UnitCountName, descriptions.UnitCountDescription); });
            speedButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.SpeedName, descriptions.SpeedDescription); });
            damageButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.DamageName, descriptions.DamageDescription); });
            attackButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.AttackName, descriptions.AttackDescription); });
            defenseButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.DefenseName, descriptions.DefenseDescription); });
            rangeButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.RangeName, descriptions.RangeDescription); });
            movementPenaliesButton.onClick.AddListener(() =>
            { descriptionPanel.ShowDescription(descriptions.MovementPenaliesName, descriptions.MovementPenaliesDescription); });
        }

        public void ShowPanel(Unit selectedUnit)
        {
            foreach (RectTransform icon in icons) icon.gameObject.SetActive(false);
            foreach (RectTransform movementPenaltyUI in movementPenaltyUIList) movementPenaltyUI.gameObject.SetActive(false);

            unitDescription.text = selectedUnit.UnitDescription;

            healthbarImage.fillAmount = (float)selectedUnit.UnitStats.Health / selectedUnit.BaseUnitStats.Health;
            healthbarText.text = selectedUnit.UnitStats.Health + "/" + selectedUnit.BaseUnitStats.Health;

            unitCountText.text = selectedUnit.UnitStats.UnitCount + "/" + selectedUnit.BaseUnitStats.UnitCount;
            speedText.text = selectedUnit.UnitStats.Speed.ToString();
            damageText.text = selectedUnit.UnitStats.Damage.ToString();
            attackText.text = selectedUnit.UnitStats.Attack.ToString();
            defenseText.text = selectedUnit.UnitStats.Defense.ToString();
            rangeText.text = selectedUnit.UnitStats.Range.ToString();
            

            for (int i = 0; i < selectedUnit.MovementPenalties.Count; i++)
            {
                RectTransform newMovementPenaltyUI;
                if (i >= movementPenaltyUIList.Count)
                {
                    newMovementPenaltyUI = Instantiate(movementPenaltyUIPrefab, movementPenaltiesUI);
                    newMovementPenaltyUI.anchoredPosition = new Vector2(movementPenaltyUIWidth * i + awayFromOutlineOffset, 0);
                    movementPenaltyUIList.Add(newMovementPenaltyUI);
                }

                movementPenaltyUIList[i].GetComponent<MovementPenaltyUI>().SetMovemetPenaltyUI(
                    selectedUnit.MovementPenalties.ElementAt(i).Key,
                    selectedUnit.MovementPenalties.ElementAt(i).Value.IsNotWalkable,
                    selectedUnit.MovementPenalties.ElementAt(i).Value.MovementExtraCost);

                movementPenaltyUIList[i].gameObject.SetActive(true);
            }

            if (selectedUnit.EffectsWithParameters == null || selectedUnit.EffectsWithParameters.Count <= 0)
            {
                for (int i = 0; i < selectedUnit.InnateEffects.Count; i++)
                {
                    RectTransform newIcon;
                    if (i >= icons.Count)
                    {
                        newIcon = Instantiate(effectIconPrefab, effectsRectTransform);
                        newIcon.anchoredPosition = new Vector2(effectIconWidth * i, 0);
                        icons.Add(newIcon);
                    }

                    icons[i].GetComponent<EffectIcon>().SetIcon(this, selectedUnit.InnateEffects[i], "\u221E");

                    icons[i].gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < selectedUnit.EffectsWithParameters.Count; i++)
            {
                RectTransform newIcon;
                if (i >= icons.Count)
                {
                    newIcon = Instantiate(effectIconPrefab, effectsRectTransform);
                    newIcon.anchoredPosition = new Vector2(effectIconWidth * i, 0);
                    icons.Add(newIcon);
                }

                string cooldown = selectedUnit.EffectsWithParameters[i].Cooldown.ToString();
                if (selectedUnit.EffectsWithParameters[i].IsUnlimitedDuration) cooldown = "\u221E";

                icons[i].GetComponent<EffectIcon>().SetIcon(this, selectedUnit.EffectsWithParameters[i].Effect, cooldown);

                icons[i].gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }

        public void ShowDescriptionPanel(string header, string description)
        {
            descriptionPanel.ShowDescription(header, description);
        }
    }
}
