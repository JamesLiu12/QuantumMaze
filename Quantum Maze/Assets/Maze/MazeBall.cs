using UnityEngine;

namespace Maze
{
    public class MazeBall : MonoBehaviour
    {
        public Vector3 destination;
        public bool moving;
        public float speed = 2.0f;
        public float epsilon = 0.1f;

        void FixedUpdate()
        {
            if (!moving) return;
        
            Vector3 direction = destination - transform.localPosition;
            if (direction.magnitude < epsilon)
            {
                transform.localPosition = destination;
                Stop();
                return;
            }

            Vector3 normalizedDirection = direction.normalized;
            transform.position += normalizedDirection * (speed * Time.deltaTime);
        }

        public void Move()
        {
            moving = true;
        }

        public void Stop()
        {
            moving = false;
        }
    }
}
