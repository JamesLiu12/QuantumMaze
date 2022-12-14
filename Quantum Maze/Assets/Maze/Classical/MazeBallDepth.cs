using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeBallDepth : MazeBall
    {
        public static readonly float baseVelocity = 10.0f;
        public static readonly float Epsilon = 0.05f;
        void FixedUpdate()
        {
            if (!moving) return;

            Vector3 direction = destination - transform.localPosition;
            if (direction.magnitude < Epsilon)
            {
                transform.localPosition = destination;
                return;
            }

            Vector3 normalizedDirection = direction.normalized;
            transform.localPosition += normalizedDirection * (baseVelocity * Time.deltaTime);
        }
    }
}
