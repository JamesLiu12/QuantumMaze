using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeViewerWidth : MonoBehaviour
    {
        public int width = 31;
        public int height = 31;
        public GameObject wallStyle;
        public GameObject ballStyle;
        private MazeWidth _maze;
        private float _startTime;
        private Dictionary<BallInfo, GameObject> _ballInfoToGameObject = new();
        
        void Start()
        {
            Demonstration();
        }
        
        private void Demonstration()
        {
            _maze = new(width, height);
            _maze.GenerateByPrims();
            _maze.CreateBallQueue();
            BuildWalls();
            FitScreen();
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
            while (_maze.BallAppearInfoQueue.Count > 0 &&
                   _maze.BallAppearInfoQueue[0].TimeAppear <= Time.fixedTime - _startTime)
            {
                BallInfo thisAppearInfo = _maze.BallAppearInfoQueue[0];
                _maze.BallAppearInfoQueue.RemoveAt(0);
                GameObject newBall = Instantiate(ballStyle, transform, false);
                newBall.transform.localPosition = upperLeftPosition +
                                                  new Vector2(thisAppearInfo.AppearPosition.Col,
                                                      -thisAppearInfo.AppearPosition.Row);
                newBall.GetComponent<MazeBall>().destination = upperLeftPosition +
                                                               new Vector2(thisAppearInfo.DeletePosition.Col,
                                                                   -thisAppearInfo.DeletePosition.Row);
                newBall.GetComponent<MazeBall>().Move();
                _ballInfoToGameObject.Add(thisAppearInfo, newBall);
            }

            while (_maze.BallDeleteInfoQueue.Count > 0)
            {
                if (_maze.BallDeleteInfoQueue[0].TimeDelete > Time.fixedTime - _startTime) break;
                BallInfo thisDeleteInfo = _maze.BallDeleteInfoQueue[0];
                _maze.BallDeleteInfoQueue.RemoveAt(0);
                GameObject ballDeleted = _ballInfoToGameObject[thisDeleteInfo];
                Destroy(ballDeleted);
            }
        }
    }
}
