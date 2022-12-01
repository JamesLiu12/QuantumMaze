using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeBallSmooth : MazeBall
    {
        public static readonly float BaseVelocity = 2.0f;
        public static readonly float Epsilon = 0.1f;
        private Vector3 GetVelocity() => (destination - transform.localPosition) * BaseVelocity;
        
        void FixedUpdate()
        {
            if (!moving) return;
            
            Vector3 positionDifference = destination - transform.localPosition;
            if (positionDifference.magnitude <= Epsilon) return;

            Vector3 velocity = positionDifference * BaseVelocity;
            transform.localPosition += velocity * Time.deltaTime;
        }
    }
}
