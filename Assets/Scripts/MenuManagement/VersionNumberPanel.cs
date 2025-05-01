using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SimpleWargame.MenuManagement
{
    public class VersionNumberPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionNumberText;

        private void Start()
        {
            versionNumberText.text = "Prototype v" + Application.version;
        }
    }
}
