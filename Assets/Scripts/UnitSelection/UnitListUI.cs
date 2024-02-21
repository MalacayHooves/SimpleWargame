using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleWargame.Units;
using System;

namespace SimpleWargame.UnitSelection
{
    public class UnitListUI : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [Header("Links to Objects")]
        [SerializeField] private UnitUIElement unitUIElementPrefab;
        [SerializeField] private Transform panel;
        [SerializeField] protected List<Unit> units;

        public event EventHandler OnClickUnitListUI;

        protected virtual void Start()
        {
            if (units == null) return;
            for (int i = 0; i < units.Count; i++)
            {
                UnitUIElement unitUIElement = Instantiate(unitUIElementPrefab);

                unitUIElement.transform.SetParent(panel, false);

                unitUIElement.SetUnit(units[i]);
                unitUIElement.SetUnitListUI(this);
                units[i] = unitUIElement.Unit;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            UnitUIElement unitUIElement = eventData.pointerDrag.GetComponent<UnitUIElement>();

            if (unitUIElement == null) return;

            if (unitUIElement.transform.parent == panel) return;

            unitUIElement.MoveToAnotherUnitListUI(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickUnitListUI?.Invoke(this, EventArgs.Empty);
        }

        public virtual void AddUnitToList(UnitUIElement unitUIElement)
        {
            unitUIElement.SetUnitListUI(this);
            unitUIElement.transform.SetParent(panel.transform, false);
            units.Add(unitUIElement.Unit);
        }

        public virtual void RemoveUnitFromList(UnitUIElement unitUIElement)
        {
            if (!units.Contains(unitUIElement.Unit)) { Debug.LogError("UnitListUI Error: list doesn't contain unit"); return; }

            units.Remove(unitUIElement.Unit);
        }
    }
}
