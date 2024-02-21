using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame.UnitUI
{
    public class UnitUI : MonoBehaviour
    {
        [Header("Links to Objects")]
        [SerializeField] private TopPanel topPanel;
        [SerializeField] private AbilityPanel abilityPanel;
        [SerializeField] private AbilityDescriptionsPanel abilityDescriptionsPanel;
        [SerializeField] private Button openPausePanelButton;
        [SerializeField] private GameObject pausePanel;

        ICanSendSelectedUnit manager;

        private void Start()
        {
            if (openPausePanelButton == null) { Debug.LogError("UnitUI Error: there is no OpenPausePanelButton"); return; }

            openPausePanelButton.onClick.AddListener(() =>
            {
                pausePanel.SetActive(true);
            });

            pausePanel.SetActive(false);
        }

        public void SubscribeToManagerEvents()
        {
            manager = FindObjectsOfType<MonoBehaviour>().OfType<ICanSendSelectedUnit>().FirstOrDefault();

            if (manager == null) { Debug.LogError("UnitUI: there is no Manager able to send unit stats"); return; }

            topPanel.SubscribeToManagerEvents(manager);
            abilityPanel.SubscribeToManagerEvents(manager);
            abilityDescriptionsPanel.SubscribeToManagerEvents(manager);
        }

        public void UnsubscribeFromManagerEvents()
        {
            if (manager == null) { Debug.LogError("UnitUI: there is no Manager able to send unit stats"); return; }

            topPanel.UnsubscribeFromManagerEvents(manager);
            abilityPanel.UnsubscribeFromManagerEvents(manager);
            abilityDescriptionsPanel.UnsubscribeFromManagerEvents(manager);
        }
    }
}
