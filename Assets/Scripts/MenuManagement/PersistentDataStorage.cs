using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.Units;

namespace SimpleWargame.MenuManagement
{
    public class PersistentDataStorage : MonoBehaviour
    {
        public static PersistentDataStorage Instance { get; private set; }

        public string MapToLoadName { get; set; }

        public List<Unit> Units { get; private set; }

        private void Start()
        {
            if (PersistentDataStorage.Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SetUnits(List<Unit> units) { Units = units; }

        public void ClearUnits()
        {
            if (Units == null) return;
            Units.Clear();
            Units.TrimExcess();
        }
    }
}
