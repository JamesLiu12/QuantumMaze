using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class FromInfo : Tuple<Cell, Cell, float>
    {
        public Cell Position => Item1;
        public Cell FromPosition => Item2;
        public float TimeAtPos => Item3;

        public FromInfo(Cell position, Cell fromPosition, float timeAtPos)
            : base(position, fromPosition, timeAtPos) { }
    }

    public class MazeWidth : Maze
    {
        public List<BallInfo> BallAppearInfoQueue = new();
        public List<BallInfo> BallDeleteInfoQueue = new();
        
        public MazeWidth(int width, int height) : base(width, height) { }

        private float GetMoveTime(Cell startPos, Cell endPos)
            => Cell.Distance(startPos, endPos) / MazeBallWidth.BaseVelocity;

        public void CreateBallQueue()
        {
            float timeAcc = 0;
            Queue<FromInfo> bfsQueue = new();
            List<List<bool>> visited = ListUtility.List2D(Height, Width, false);
            bfsQueue.Enqueue(new(StartPos + Cell.Right, StartPos,0));
            while (bfsQueue.Count > 0)
            {
                FromInfo preMoveInfo = bfsQueue.Dequeue();
                Cell preStartCell = preMoveInfo.Position;
                Cell preFromCell = preMoveInfo.FromPosition;
                float preMoveTime = preMoveInfo.TimeAtPos;
                BallAppearInfoQueue.Add(new(preFromCell, preStartCell, 
                    preMoveTime, timeAcc));
                foreach (var neighbour in Cell.Neighbours)
                {
                    Cell newCell = preStartCell + neighbour;
                    if (OutOfBound(newCell) || Blocked(newCell) || newCell.Equals(preFromCell)) continue;
                    bfsQueue.Enqueue(new(newCell, preStartCell, timeAcc));
                    timeAcc += 1 / MazeBallWidth.BaseVelocity;
                }
            }
            foreach (var ballInfo in BallAppearInfoQueue) 
                if (!ballInfo.DeletePosition.Equals(EndPos))
                    BallDeleteInfoQueue.Add(ballInfo);
        }
    }
}
