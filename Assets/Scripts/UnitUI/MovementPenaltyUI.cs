using SerializableDictionary.Scripts;
using SimpleWargame.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.UnitUI
{
    public class MovementPenaltyUI : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] Image tileImage;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject noMovementImage;
        [SerializeField] private SerializableDictionary<ENUM_TileType, Sprite> tiles;

        public void SetMovemetPenaltyUI(ENUM_TileType tileType, bool isNotWalkable, int movementCost)
        {
            if (tiles.Dictionary.TryGetValue(tileType, out Sprite sprite)) tileImage.sprite = sprite;
            else { Debug.LogError("MovementPenaltyUI Error: UI Dictionary doesn't contain such tile type!"); return; }
            noMovementImage.SetActive(isNotWalkable);
            text.gameObject.SetActive(!isNotWalkable);
            text.text = movementCost.ToString();
        }
    }
}
