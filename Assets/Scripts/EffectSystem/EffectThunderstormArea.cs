using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.EffectSystem
{
    [CreateAssetMenu(fileName = "NewEffectThunderstormArea", menuName = "SimpleWargame/Effects/EffectThunderstormArea")]
    public class EffectThunderstormArea : Effect
    {
        [SerializeField] private int rangePenalty;
        [SerializeField] private int attackPenalty;

        public override void Activate(Unit unit)
        {
            base.Activate(unit);

            if (unit.EffectsWithParameters.Exists(x => x.Effect.GetType() == typeof(EffectRangedWeapons)))
            {
                UnitStats unitStats = unit.UnitStats;

                unitStats.Range -= rangePenalty;
                if (unitStats.Range < 1) unitStats.Range = unit.UnitStats.Range;
                
                unitStats.Attack -= attackPenalty;
                if (unitStats.Attack < 0) unitStats.Attack = 0;

                unit.ChangeUnitsStats(unitStats);
            }
        }
    }
}
