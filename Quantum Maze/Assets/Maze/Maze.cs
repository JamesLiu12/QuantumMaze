using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int Width = 10;
    public int Height = 10;
    public List<List<bool>> Grid = new();

    public void Generate()
    {
        for (int h = 0; h < Height; h += 1)
        {
            List<bool> newRow = new();
            for (int w = 0; w < Width; w += 1)
                newRow.Add(false);
            Grid.Add(newRow);
        }

        // TODO
    }

    public void Randomize()
    {
        for (int h = 0; h < Height; h += 1)
            for (int w = 0; w < Width; w += 1)
                Grid[h][w] = Random.Range(0.0f, 1.0f) > 0.5f;
    }
}
