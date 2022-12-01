using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace Maze
{
    public class BallInfo : Tuple<Cell, Cell, float, float>
    {
        public Cell AppearPosition => Item1;
        public Cell DeletePosition => Item2;
        public float TimeAppear => Item3;
        public float TimeDelete => Item4;
        
        public BallInfo(Cell appearPosition, Cell deletePosition, float timeAppear, float timeDelete) 
            : base(appearPosition, deletePosition, timeAppear, timeDelete) { }
    }
    
    public class MoveInfo : Tuple<Cell, Cell, float>
    {
        public Cell Position => Item1;

        public Cell MoveDir => Item2;
        public float TimeAtPos => Item3;
        
        public MoveInfo(Cell deletePosition, Cell moveDir, float timeDelete) 
            : base(deletePosition, moveDir, timeDelete) { }
    }
    
    public class MazeSmooth : Maze
    {
        public List<BallInfo> BallAppearInfoQueue;
        public List<BallInfo> BallDeleteInfoQueue;

        public MazeSmooth(int width, int height) : base(width, height) { }
        
        public float GetMoveTime(Cell startPos, Cell endPos, Cell midPos)
        {
            float wholeDistance = Cell.Distance(startPos, endPos);
            float generateDistance = Cell.Distance(startPos, midPos);
            float baseVelocity = MazeBallSmooth.BaseVelocity;
            float epsilon = MazeBallSmooth.Epsilon;
            return 1 / baseVelocity * Mathf.Log(wholeDistance / (wholeDistance - generateDistance + epsilon));
        }

        public void CreateBallQueue()
        {
            BallAppearInfoQueue = new();
            BallDeleteInfoQueue = new();
            List<List<bool>> visited = ListUtility.List2D(Height, Width, false);
            Queue<MoveInfo> bfsQueue = new();
            bfsQueue.Enqueue(new(StartPos, Cell.Right, 0));
            while (bfsQueue.Count > 0)
            {
                MoveInfo preMoveInfo = bfsQueue.Dequeue();
                Cell preStartCell = preMoveInfo.Position;
                Cell preMoveDir = preMoveInfo.MoveDir;
                float preMoveTime = preMoveInfo.TimeAtPos;
                Cell preFromDir = new(-preMoveDir.Row, -preMoveDir.Col);
                Cell preDestination = FindDestination(preStartCell, preMoveDir);
                GenerateBallsinPath(preMoveInfo, preDestination, bfsQueue);
                float preTimeNeeded = GetMoveTime(preStartCell, preDestination, preDestination);
                BallAppearInfoQueue.Add(new(preStartCell, preDestination, preMoveTime, preMoveTime + preTimeNeeded));

                int numberWays = 0;
                foreach (var neighbour in Cell.Neighbours)
                {
                    Cell newCell = preDestination + neighbour;
                    if (!OutOfBound(newCell) && !Blocked(newCell)) numberWays++;
                }
                if (numberWays > 1)
                    BallDeleteInfoQueue.Add(new(preStartCell, preDestination, preMoveTime, preMoveTime + preTimeNeeded));
            }
            BallAppearInfoQueue.Sort(new ISortBallInfoByAppearTime());
            BallDeleteInfoQueue.Sort(new ISortBallInfoByDeleteTime());
        }

        public void GenerateBallsinPath(MoveInfo startMoveInfo, Cell destination, Queue<MoveInfo> bfsQueue)
        {
            Cell startCell = startMoveInfo.Position;
            Cell moveDir = startMoveInfo.MoveDir;
            float startTime = startMoveInfo.TimeAtPos;
            Cell curCell = startCell + moveDir;
            Cell fromDir = new(-moveDir.Row, -moveDir.Col);
            while (!Equals(curCell, EndPos) && !OutOfBound(curCell) && !Blocked(curCell))
            {
                float timeNeeded = GetMoveTime(startCell, destination, curCell);
                foreach (var neighbour in Cell.Neighbours)
                {
                    if (Equals(neighbour, fromDir) || Equals(neighbour, moveDir)) continue;
                    Cell newCell = curCell + neighbour;
                    if (OutOfBound(newCell) || Blocked(newCell)) continue;
                    bfsQueue.Enqueue(new(curCell, neighbour, startTime + timeNeeded));
                }
                curCell += moveDir;
            }
        }
        
        
        public Cell FindDestination(Cell startCell, Cell moveDir)
        {
            Cell pos = startCell + moveDir;
            while (!Equals(pos, EndPos) && !OutOfBound(pos + moveDir) && !Blocked(pos + moveDir)) 
                pos += moveDir;
            return pos;
        }
    }

    public class ISortBallInfoByAppearTime : IComparer<BallInfo>
    {
        public int Compare(BallInfo a, BallInfo b) 
            => a.TimeAppear.CompareTo(b.TimeAppear);
    }

    public class ISortBallInfoByDeleteTime: IComparer<BallInfo>
    {
        public int Compare(BallInfo a, BallInfo b)
            => a.TimeDelete.CompareTo(b.TimeDelete);
    }
}
