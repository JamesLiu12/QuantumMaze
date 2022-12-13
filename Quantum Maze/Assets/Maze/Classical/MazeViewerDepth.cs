using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeViewerDepth : MonoBehaviour
    {
        public int width = 31;
        public int height = 31;
        public GameObject wallStyle;
        public GameObject ballStyle;
        private MazeDepth _maze;
        private float _startTime;
        private GameObject _ballObject;
        private bool _arrived = false;
        private float lastMoveTime = 0;
        void Start()
        {
            Demonstration();
        }
        
        private void Demonstration()
        {
            _maze = new(width, height);
            _maze.GenerateByPrims();
            _maze.CreatePathQueue();
            BuildWalls();
            FitScreen();
            Vector2 upperLeftPosition = new(
                - 0.5f * (_maze.Height - 1),
                + 0.5f * (_maze.Width - 1));
            _ballObject = Instantiate(ballStyle, transform, false);
            _ballObject.transform.localPosition = upperLeftPosition + new Vector2(0, -1);
            _startTime = Time.fixedTime;
            
        }
        private void FitScreen()
        {
            transform.localScale *= 0.9f * 19.5f / height;
        }
        
        private void BuildWalls()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            Vector2 upperLeftPosition = new(
                - 0.5f * (_maze.Height - 1),
                + 0.5f * (_maze.Width - 1));

            for (int h = 0; h < _maze.Height; h += 1)
            for (int w = 0; w < _maze.Width; w += 1)
                if (_maze.Grid[h][w])
                {
                    GameObject newWall = Instantiate(wallStyle, transform, true);
                    newWall.transform.position = upperLeftPosition + new Vector2(w, -h);
                }
        }
        
        void Update()
        {
            CheckQueues();
        }

        void CheckQueues()
        {
            Vector2 upperLeftPosition = new(
                - 0.5f * (_maze.Height - 1),
                + 0.5f * (_maze.Width - 1));
            while (_maze.PathQueue.Count > 0 &&
                   _maze.PathQueue[0].TimeAtPos <= Time.fixedTime - _startTime)
            {
                lastMoveTime = Time.fixedTime;
                PathInfo thisPathInfo = _maze.PathQueue[0];
                _maze.PathQueue.RemoveAt(0);
                var ballComponent = _ballObject.GetComponent<MazeBall>();
                if (!ballComponent.destination.Equals(new(0, 0, 0)))
                    _ballObject.transform.localPosition = ballComponent.destination;
                ballComponent.destination =
                    upperLeftPosition + new Vector2(thisPathInfo.Position.Col, -thisPathInfo.Position.Row);
                ballComponent.Move();
            }
            if (_maze.PathQueue.Count == 0 && Time.fixedTime - lastMoveTime >= 1 / MazeBallDepth.baseVelocity)
                if (!_arrived)
                {
                    _arrived = true;
                    var ballComponent = _ballObject.GetComponent<MazeBall>();
                    ballComponent.transform.localPosition = ballComponent.destination;
                    ballComponent.Stop();
                }
        }
    }
}
