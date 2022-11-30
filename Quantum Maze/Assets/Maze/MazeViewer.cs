using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Maze
{
    public class MazeViewer : MonoBehaviour
    {
        public int width = 31;
        public int height = 31;
        public GameObject wallStyle;
        public GameObject ballStyle;
        public float speed = 2.0f;

        public enum BallVisibility
        {
            All, 
            ShowBallsCannotMove, 
            ShowOnlyMovingBalls
        }

        public BallVisibility ballVisibility = BallVisibility.ShowBallsCannotMove;

        private Maze _maze;
        private readonly Queue<Tuple<GameObject, Cell>> _currentMovements = new();
        private List<List<bool>> _walked = new();
        private Cell _destinationCell;

        private void Start()
        {
            Demonstration();
        
        }

        private void Update()
        {
            // Take a step when ENTER is pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // ReSharper disable Unity.PerformanceCriticalCodeInvocation
                Step();
                // ReSharper restore Unity.PerformanceCriticalCodeInvocation
            }
        }

        private void Initialise()
        {
            _currentMovements.Clear();
            _walked = ListUtility.List2D(height, width, false);
            _destinationCell = new(height-2, width-1);
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
    
        /// <summary>
        /// Resize the maze so that it fits the screen
        /// </summary>
        private void FitScreen()
        {
            transform.localScale *= 0.9f * 19.5f / height;
        }

        private Vector3 LocalPositionFrom(Cell cell) => new Vector3(
            cell.Col - width * 1.0f / 2 + 0.5f,
            -cell.Row + height * 1.0f / 2 - 0.5f);

        private void Demonstration()
        {
            Initialise();
        
            _maze = new(width, height);
            _maze.GenerateByPrims();
            BuildWalls();
            FitScreen();
        
            // Create the new ball
            // Start from the upper right, and the first step is to the right.
            var initialBall = 
                Instantiate(ballStyle, transform, false)
                    .GetComponent<MazeBall>();
            initialBall.speed = speed;
            var initialCell = new Cell(1, 0);
            var initialCellNext = new Cell(1, 1);
        
            // Set its start position and destination
            initialBall.transform.localPosition = LocalPositionFrom(initialCell);
            initialBall.destination = LocalPositionFrom(initialCellNext);

            _currentMovements.Enqueue(new(initialBall.gameObject, initialCellNext));
        
            _walked[initialCell.Row][initialCell.Col] = true;
            _walked[initialCellNext.Row][initialCellNext.Col] = true;
        
            // Make it move
            initialBall.Move();
        }

        /// <summary>
        /// Let all balls take a single step
        /// This can be invoked freely by others
        /// </summary>
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public void Step()
        {
            // Stop if there is no more step to be taken or
            // there is a ball reached the destination
            if (_currentMovements.Count == 0 ||
                _walked[_destinationCell.Row][_destinationCell.Col])
                return;
        
            // All balls present move in a step
            int lastMovementsCount = _currentMovements.Count;
            for (int _ = 0; _ < lastMovementsCount; _++)
            {
                // Take the old ball and destroy it
                var oldBallMovement = _currentMovements.Dequeue();
            
                // Destroy the ball depends on the visibility setting
                switch (ballVisibility)
                {
                    case BallVisibility.ShowOnlyMovingBalls:
                        Destroy(oldBallMovement.Item1); break;
                
                    // 3 Walls and 1 walked cell around shows
                    // the ball cannot move
                    case BallVisibility.ShowBallsCannotMove:
                        int wallCount = 0;
                        int walkedCount = 0;
                    
                        foreach (var neighbour in Cell.Neighbours)
                        {
                            var neighbourCell = oldBallMovement.Item2 + neighbour;
                            if (_maze.OutOfBound(neighbourCell))
                                continue;
                            if (_maze.Blocked(neighbourCell))
                                wallCount += 1;
                            if (_walked[neighbourCell.Row][neighbourCell.Col])
                                walkedCount += 1;
                        }

                        if (wallCount != 3 || walkedCount != 1)
                            Destroy(oldBallMovement.Item1);
                        break;

                    case BallVisibility.All: break;
                }
            
                // Any new ball will start at the old ball's destination
                var startCell = oldBallMovement.Item2;
            
                // Check 4 neighbour cells
                foreach (var neighbourCell in Cell.Neighbours)
                {
                    var nextCell = startCell + neighbourCell;
                
                    // Ignore any cell walked or in the wall
                    if (_maze.Blocked(nextCell) || 
                        _walked[nextCell.Row][nextCell.Col]) 
                        continue;
                
                    // Create the new ball
                    var newBall = Instantiate(ballStyle, transform, false).GetComponent<MazeBall>();
                    newBall.speed = speed;
                
                    // Set its start position and destination
                    newBall.transform.localPosition = LocalPositionFrom(startCell);
                    newBall.destination = LocalPositionFrom(nextCell);
                
                    _currentMovements.Enqueue(new(newBall.gameObject, nextCell));

                    _walked[nextCell.Row][nextCell.Col] = true;
                
                    // Make it move
                    newBall.Move();
                }
            }
        }
    }
}
