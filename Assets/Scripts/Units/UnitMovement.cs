using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.Units
{
    public class UnitMovement : MonoBehaviour
    {
        private float speed;
        private Unit unit;

        public void SetUnit(Unit unit) { this.unit = unit; }

        public async Task MoveUnit(Vector3 moveTo, List<Vector3> path, float speed)
        {
            if (unit == null) { Debug.LogError("UnitMovement Error: Unit == null"); return; }

            this.speed = speed;
            for (int i = 0; i < path.Count; i++)
            {
                await MoveTo(path[i]);
            }

            if (unit != null) unit.FinishMovement();
        }

        private async Task MoveTo(Vector3 endPosition)
        {
            Vector3 startPosition = transform.position;
            float startTime = Time.time;
            float moveDistance = Vector3.Distance(transform.position, endPosition);
            float distanceCovered = 0;

            while (distanceCovered < moveDistance)
            {
                distanceCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(startPosition, endPosition, distanceCovered / moveDistance);

                await Task.Yield();
            }

            transform.position = endPosition;
        }
    }
}
