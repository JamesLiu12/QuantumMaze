using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace Maze
{
    public class PathInfo : Tuple<Cell, float>
    {
        public Cell Position => Item1;
        public float TimeAtPos => Item2;

        public PathInfo(Cell position, float timeAtPos)
            : base(position, timeAtPos) { }
    }
    
    public class MazeDepth : Maze
    {
        public List<PathInfo> PathQueue = new();
        public MazeDepth(int width, int height) : base(width, height) { }

        public void CreatePathQueue()
        {
            List<Cell> movePath = new();
            DfsFindPath(movePath, StartPos, new Cell(0, 0));
            PathQueue.Add(new(StartPos, 0));
            for (int i = 0; i < movePath.Count; i++)
            {
                PathQueue.Add(new(PathQueue.Last().Position +  movePath[i],
                    i / MazeBallDepth.baseVelocity));
            }
            PathQueue.RemoveAt(0);
            while (!PathQueue.Last().Position.Equals(EndPos)) PathQueue.RemoveAt(PathQueue.Count - 1);
        }

        private void DfsFindPath(List<Cell> movePath, Cell startCell, Cell fromDir)
        {
            foreach (var neighbour in Cell.Neighbours)
            {
                Cell newCell = startCell + neighbour;
                if (neighbour.Equals(fromDir) || OutOfBound(newCell) || Blocked(newCell)) continue;
                movePath.Add(neighbour);
                DfsFindPath(movePath, newCell, -neighbour);
            }
            if (fromDir.Row != 0 || fromDir.Col != 0) movePath.Add(fromDir);
        }
    }
}
