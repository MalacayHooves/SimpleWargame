using SimpleWargame.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace SimpleWargame.Input
{
    /// <summary>
    /// This class handles clicking on map tiles
    /// </summary>
    public class MapInput : MonoBehaviour, IPointerClickHandler, IDropHandler
    {
        [Header("Links to Objects")]
        [SerializeField] private MapManager mapManager;


        private void Awake()
        {
            if (mapManager == null) mapManager = GetComponent<MapManager>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (mapManager == null) return;
            mapManager.MapClicked(eventData.pointerPressRaycast.worldPosition);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (mapManager == null) return;

            mapManager.MapClicked(eventData.pointerCurrentRaycast.worldPosition);
        }
    }
}