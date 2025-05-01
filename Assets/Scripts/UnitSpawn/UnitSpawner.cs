using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWargame.MenuManagement;
using SimpleWargame.Units;
using SimpleWargame.BattleManagement;

namespace SimpleWargame.UnitSpawn
{
    public class UnitSpawner : MonoBehaviour
    {
        public static UnitSpawner Instance { get; private set; }

        [Header("Links to Objects")]
        [SerializeField] private Transform panel;
        [SerializeField] private GameObject mapBlocker;
        [SerializeField] private Player player;

        private List<Unit> units = new List<Unit>();

        private void Start()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (PersistentDataStorage.Instance == null) { Debug.LogError("UnitSpawnUI Error: there is no PersistentDataStorage"); return; }
            if (PersistentDataStorage.Instance.Units == null) return;

            Unit spawnedUnit;

            foreach (Unit unit in PersistentDataStorage.Instance.Units)
            {
                spawnedUnit = Instantiate(unit, panel);
                units.Add(spawnedUnit);
            }

            PersistentDataStorage.Instance.ClearUnits();

            if (BattleManager.Instance == null) { Debug.LogError("UnitSpawnUI Error: there is no BattleManagewr"); return; }
        }

        public void MoveUnitToPlayer(Unit unit)
        {
            if (!units.Contains(unit)) { Debug.LogError("UnitSpawnUI Error: list of ready to spawn units doesn't contain this one"); return; }

            unit.transform.SetParent(player.transform, true);

            units.Remove(unit);

            if (units.Count < 1)
            {
                BattleManager.Instance.FinishUnitSpawn();
                mapBlocker.SetActive(false);
            }
        }
    }
}
