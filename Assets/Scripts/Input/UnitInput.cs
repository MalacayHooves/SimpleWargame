using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleWargame.Units;

namespace SimpleWargame.Input
{
    /// <summary>
    /// This class handles clicking on units and dragging them
    /// </summary>
    public class UnitInput : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Unit unit;

        private void Start()
        {
            unit = GetComponent<Unit>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging) unit.ClickUnit();
        }

        public void OnDrag(PointerEventData eventData)
        {
            unit.OnDrag(eventData.pointerCurrentRaycast.worldPosition);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            unit.BeginDrag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            unit.EndDrag();
        }
    }
}
