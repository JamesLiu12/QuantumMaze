using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeViewer : MonoBehaviour
{
    public int Width = 19;
    public int Height = 19;
    public GameObject WallStyle;
    public List<MazeBall> Balls;

    private Maze MazeToView;

    void Start()
    {
        Test1();
    }
    private void Test1()
    {
        MazeToView = new(Width, Height);
        MazeToView.GenerateByPrims();
        BuildWalls();
        FitScreen();
    }

    public void BuildWalls()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        Vector2 upperLeftPosition = new(
            - 0.5f * (float)(MazeToView.Height - 1),
            + 0.5f * (float)(MazeToView.Width - 1));

        for (int h = 0; h < MazeToView.Height; h += 1)
        for (int w = 0; w < MazeToView.Width; w += 1)
        if (MazeToView.Grid[h][w])
        {
            GameObject newWall = Instantiate(WallStyle, transform, true);
            newWall.transform.position = upperLeftPosition + new Vector2(w, -h);
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
}
