using SimpleWargame.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleWargame.UnitUI
{
    public interface ICanSendSelectedUnit
    {
        public event EventHandler OnSelectUnit;

        public Unit SelectedUnit { get; }

        public void UnsubscribeUIFromManagerEvents();
    }
}
