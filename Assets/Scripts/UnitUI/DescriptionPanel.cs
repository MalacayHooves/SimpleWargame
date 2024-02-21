using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.UnitUI
{
    public class DescriptionPanel : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] protected Button closeButton;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });

            gameObject.SetActive(false);
        }

        public void ShowDescription(string header, string description)
        {
            headerText.text = header;
            descriptionText.text = description;
            gameObject.SetActive(true);
        }
    }
}
