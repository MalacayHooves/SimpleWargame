using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleWargame.UnitSelection
{
    public class UnitUIElement: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Links to Objects")]
        [SerializeField] private GameObject draggableImageGO;
        [SerializeField] private GameObject selectedImageGO;

        [Header("Variables")]
        [SerializeField] private int timeToHoldBeforeShowDraggableElement;

        private CancellationTokenSource taskToken;
        private Canvas canvas;
        private Unit unit;
        private UnitListUI unitListUI;
        private UnitListUI[] unitListUIArray;

        public Unit Unit => unit;

        private void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            draggableImageGO.transform.SetParent(canvas.transform, false);

            UnitSelectionUI.Instance.OnSelectUnit += UnitSelectionUI_OnSelectUnit;

            unitListUIArray = FindObjectsOfType<UnitListUI>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (draggableImageGO != null)
            {
                draggableImageGO.transform.position = eventData.position;
            }
        }

        public async void OnPointerDown(PointerEventData eventData)
        {
            if (UnitSelectionUI.Instance == null) { Debug.LogError("UnitUIElement Error: there is no UnitSelectionUI"); return; }
            
            UnitSelectionUI.Instance.SelectUnit(this);

            taskToken = new CancellationTokenSource();

            await ShowDraggableElement(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (taskToken != null)
            {
                if (taskToken.IsCancellationRequested)
                    return;
                else
                    taskToken.Cancel();
            }

            draggableImageGO.SetActive(false);
        }

        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            unit.SetStatsDefaultValues();
            Sprite sprite = this.unit.UnitSprite;
            GetComponent<Image>().sprite = sprite;
            draggableImageGO.GetComponent<Image>().sprite = sprite;
        }

        public void SetUnitListUI(UnitListUI unitListUI) { this.unitListUI = unitListUI; }

        public void MoveToAnotherUnitListUI(UnitListUI unitListUI)
        {
            this.unitListUI.RemoveUnitFromList(this);
            unitListUI.AddUnitToList(this);
        }

        private async Task ShowDraggableElement(PointerEventData eventData)
        {
            await Task.Delay(timeToHoldBeforeShowDraggableElement);
            if (taskToken.IsCancellationRequested) return;
            if (eventData.IsPointerMoving()) return;
            
            draggableImageGO.SetActive(true);
            draggableImageGO.transform.position = eventData.position;

            await Task.Yield();
        }
        private void UnitSelectionUI_OnSelectUnit(object sender, EventArgs e)
        {
            if (UnitSelectionUI.Instance.SelectedUnitUIElement == this)
            {
                foreach (UnitListUI unitListUI in unitListUIArray)
                {
                    unitListUI.OnClickUnitListUI += UnitListUI_OnClickUnitListUI;
                }
                selectedImageGO.SetActive(true);
            }
            else
            {
                foreach (UnitListUI unitListUI in unitListUIArray)
                {
                    unitListUI.OnClickUnitListUI -= UnitListUI_OnClickUnitListUI;
                }
                selectedImageGO.SetActive(false);
            }
        }

        private void UnitListUI_OnClickUnitListUI(object sender, EventArgs e)
        {
            if (sender == (object)unitListUI) return;
            else
            {
                MoveToAnotherUnitListUI((UnitListUI)sender);
            }
        }
    }
}
