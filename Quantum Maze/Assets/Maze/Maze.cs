using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class Cell : Tuple<int, int>
{
    public int Row { get => Item1; }
    public int Col { get => Item2; }

    public Cell(int h, int w) : base(h, w) { }

    public static Cell operator +(Cell a, Cell b)
    => new Cell(a.Row + b.Row, a.Col + b.Col);

    public static Cell operator -(Cell a, Cell b)
    => new Cell(a.Row - b.Row, a.Col - b.Col);

    public static bool Equals(Cell a, Cell b)
    => a.Row == b.Row && a.Col == b.Col;
};

public class Maze
{
    // Width and Height must be odd integer
    public int Width;
    public int Height;
    public List<List<bool>> Grid = new();
    public Cell StartPos = new Cell(1, 0);
    public Cell EndPos;
    public List<List<bool>> visited = new();
    public Maze(int width, int height)
    {
        Width = width;
        Height = height;
        Initialise();
    }

    public void Initialise()
    {
        if (Height < 5 || Height % 2 == 0 ||
            Width < 5 || Width % 2 == 0)
        {
            throw new Exception(
                "Height and Width must be " +
                "positive odd numbers no less than 5.");
        }
        Grid = ListUtility.List2D(Height, Width, true);
        EndPos = new Cell(Height - 2, Width - 1);
        visited = ListUtility.List2D(Height, Width, false);
    }

    public void GenerateByPrims()
    {
        // Ways of movement a cell can make each time
        List<Cell> neighbours = new() {
            new(1, 0), new(-1, 0),
            new(0, 1), new(0, -1),
        };

        // Set up walls between cells
        for (int h = 1; h < Height - 1; h += 2)
            for (int w = 1; w < Width - 1; w += 2)
                Grid[h][w] = false;

        // Start with the upper left cell,
        List<List<bool>> visitedCell = ListUtility.List2D(Height, Width, false);
        visitedCell[1][1] = true;

        // Upper left cell touches the walls to the right and below
        List<Cell> currentWalls = new() { new(2, 1), new(1, 2) };

        // When there are unresolved walls remain
        while (currentWalls.Count > 0)
        {
            // Randomly pick a wall and remove it from the list
            int pickedWallIndex
                = UnityEngine.Random.Range(0, currentWalls.Count - 1);
            Cell pickedWall = currentWalls[pickedWallIndex];
            currentWalls.RemoveAt(pickedWallIndex);

            Cell? newVisitCell = null;

            if (pickedWall.Row % 2 == 0)
            // Horizontal wall --
            {
                if (!visitedCell[pickedWall.Row + 1][pickedWall.Col])
                    // Up to down
                    newVisitCell = new(pickedWall.Row + 1, pickedWall.Col);
                else
                if (!visitedCell[pickedWall.Row - 1][pickedWall.Col])
                    // Down to up
                    newVisitCell = new(pickedWall.Row - 1, pickedWall.Col);
            }
            else
            // Vertical wall   |
            {
                if (!visitedCell[pickedWall.Row][pickedWall.Col - 1])
                    // Left to right
                    newVisitCell = new(pickedWall.Row, pickedWall.Col - 1);
                else
                if (!visitedCell[pickedWall.Row][pickedWall.Col + 1])
                    // Right to left
                    newVisitCell = new(pickedWall.Row, pickedWall.Col + 1);
            }

            if (newVisitCell == null) continue;

            visitedCell[newVisitCell.Row][newVisitCell.Col] = true;

            // This wall is removed
            Grid[pickedWall.Row][pickedWall.Col] = false;

            // Add the walls next to this cell to the list
            foreach (Cell neighbour in neighbours)
            {
                Cell wallBeside = newVisitCell + neighbour;

                // This wall should exist and
                // not out of the bound
                if (!OutOfRange(wallBeside) &&
                    Grid[wallBeside.Row][wallBeside.Col])
                    currentWalls.Add(wallBeside);
            }

            //Set Entrance and Exit
            Grid[StartPos.Row][StartPos.Col] = false;
            Grid[EndPos.Row][EndPos.Col] = false;
        }
    }

    public Cell FindDestination(Cell startCell, Cell moveDir)
    {
        Cell pos = startCell;
        for (pos = startCell + moveDir; 
            !Equals(pos, EndPos) && !Blocked(pos + moveDir); pos += moveDir) 
        {
            visited[pos.Row][pos.Col] = true;
        }
        return pos;
    }

    private bool Blocked(Cell cell) => Grid[cell.Row][cell.Col];

    private bool OutOfRange(Cell node) =>
        node.Row <= 0 ||
        node.Row >= Height - 1 ||
        node.Col <= 0 ||
        node.Col >= Width - 1;

    public void Randomize()
    {
        for (int h = 0; h < Height; h += 1)
            for (int w = 0; w < Width; w += 1)
                Grid[h][w] = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f;
    }
}
