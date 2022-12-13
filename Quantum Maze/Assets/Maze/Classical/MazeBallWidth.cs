using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeBallWidth : MazeBall
    {
        public static readonly float BaseVelocity = 15.0f;
        public static readonly float Epsilon = 0.1f;
        
        void FixedUpdate()
        {
            if (!moving) return;
            
            Vector3 positionDifference = destination - transform.localPosition;
            if (positionDifference.magnitude <= Epsilon)
            {
                Stop();
                return;
            }

            Vector3 velocity = positionDifference * BaseVelocity;
            transform.localPosition += velocity * Time.deltaTime;
        }
    }
}
