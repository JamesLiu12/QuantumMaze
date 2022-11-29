using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class MazeViewer : MonoBehaviour
{
    public int Width = 31;
    public int Height = 31;
    public GameObject WallStyle;
    public GameObject BallStyle;
    public List<MazeBall> Balls;
    public Vector2 UpperLeftPosition;
    private float Epsilon = 0.05f;
    public List<List<bool>> AlreadyGenerated = new();

    private Maze MazeToView;

    void Start()
    {
        Test1();
    }

    private void Update()
    {
        //每帧都生成需要新生成的球
        GenerateBalls();
    }
    private void Test1()
    {
        AlreadyGenerated = ListUtility.List2D(Height, Width, false);
        MazeToView = new(Width, Height);
        MazeToView.GenerateByPrims();
        BuildWalls();
        FitScreen();
    }

    public void BuildWalls()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        UpperLeftPosition = new(
            - 0.5f * (MazeToView.Height - 1),
            + 0.5f * (MazeToView.Width - 1));

        for (int h = 0; h < MazeToView.Height; h += 1)
        for (int w = 0; w < MazeToView.Width; w += 1)
        if (MazeToView.Grid[h][w])
        {
            GameObject newWall = Instantiate(WallStyle, transform, true);
            newWall.transform.position = UpperLeftPosition + new Vector2(w, -h);
        }
    }

    /// <summary>
    /// Resize the maze so that it fits the screen
    /// </summary>
    public void FitScreen()
    {
        transform.localScale *= 0.9f * 19.5f / Height;
    }

    public void Step()
    {

    }

    public Cell GetCellFromVector2(Vector2 vec2) { 
        Vector2 relativePos = vec2 - UpperLeftPosition;
        return new Cell((int)Math.Round(relativePos.x), (int)Math.Round(relativePos.y));
    }

    public Vector2 GetVector2FromCell(Cell cell) => new Vector2(cell.Row + UpperLeftPosition.x, cell.Col + UpperLeftPosition.y);

    public bool CheckBallInCenter(MazeBall ball)
    {
        Vector3 postition = ball.transform.position;
        return Math.Abs(postition.x - (int)postition.x) < Epsilon && Math.Abs(postition.y - (int)postition.y) < Epsilon;
    }

    public void GenerateBallAtStartPos()
    {
        Vector2 realPos = GetVector2FromCell(MazeToView.StartPos);
        //...Instantiate at realPos
        Vector2 destination = GetVector2FromCell(MazeToView.FindDestination(MazeToView.StartPos, new Cell(0, 1)));
        Balls.Add(new MazeBall(destination));
        AlreadyGenerated[MazeToView.StartPos.Row][MazeToView.StartPos.Col] = true;
        MazeToView.visited[MazeToView.StartPos.Row][MazeToView.StartPos.Col] = true;
    }

    public void GenerateBalls()
    {
        foreach (MazeBall ball in Balls)
        {
            if (!CheckBallInCenter(ball)) continue;
            Cell thisCell = GetCellFromVector2(new Vector2(ball.transform.position.x, ball.transform.position.y));
            if (AlreadyGenerated[thisCell.Row][thisCell.Col]) continue;
            AlreadyGenerated[thisCell.Row][thisCell.Col] = true;
            List<Cell> neighbours = new() {
                new(1, 0), new(-1, 0),
                new(0, 1), new(0, -1),
            };
            //在cell的位置上生成balls
            foreach(Cell neighbour in neighbours)
            {
                Cell newCell = thisCell + neighbour;
                if (!MazeToView.Grid[newCell.Row][newCell.Col] && !MazeToView.visited[newCell.Row][newCell.Col])
                {
                    //generate new ball
                    Cell cellDes = MazeToView.FindDestination(thisCell, neighbour);
                    Vector2 destination = GetVector2FromCell(cellDes);
                    MazeBall newBall = new MazeBall(new Vector3(destination.x, destination.y, 0));
                    Balls.Add(newBall);
                    //...Instantiate at thisCell
                }
            }
        }
    }
}
