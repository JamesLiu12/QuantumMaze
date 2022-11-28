using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int posH, posW;
    public Node(int h, int w)
    {
        posH = h; posW = w;
    }
    public static Node operator +(Node a, Node b) => new Node(a.posH + b.posH, a.posW + b.posW);
    public static Node operator -(Node a, Node b) => new Node(a.posH - b.posH, a.posW - b.posW);
    public static bool Equals(Node a, Node b) => a.posH == b.posH && a.posW == b.posW;
};

public class Maze : MonoBehaviour
{
    //Width and Height must be odd integer
    public int Width = 15;
    public int Height = 15;
    public List<List<bool>> Grid = new();
    public Node StartPos = new Node(1, 1);
    public HashSet<Node> EndPos = new HashSet<Node>();
    public Maze()
    {
        InitializeMaze();
    }
    public void InitializeMaze()
    {
        Grid.Clear();
        for (int h = 0; h < Height; h += 1)
        {
            List<bool> row = new();
            for (int w = 0; w < Width; w += 1)
                row.Add(false);
            Grid.Add(row);
        }
    }
    public void GenerateMaze()
    {
        // Initialize Maze
        for (int h = 0; h < Height; h += 1)
            for (int w = 0; w < Width; w += 1)
                Grid[h][w] = !(h % 2 == 1 && w % 2 == 1);

        //Randomized Prims's Algorithm
        List<Node> edges = new();
        List<Node> dirs = new() { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
        List<List<bool>> visited = new(Height);
        foreach (List<bool> x in visited)
            for (int i = 0; i < Width; i++) x.Add(false);
        foreach (Node dir in dirs)
        {
            Node newPos = StartPos + dir;
            if (!IsOutOfRange(newPos)) edges.Add(newPos);
        }
        while (edges.Count > 0)
        {
            int edgeIndex = Random.Range(0, edges.Count - 1);
            Node edge = edges[edgeIndex];
            edges.RemoveAt(edgeIndex);
            int cnt = 0;
            foreach (Node dir in dirs)
            {

                Node newPos = edge + dir;
                if (!IsOutOfRange(newPos) && newPos.posH % 2 == 1 && newPos.posW % 2 == 1)
                {
                    if (!visited[newPos.posH][newPos.posW])
                    {
                        cnt++;
                        visited[newPos.posH][newPos.posW] = true;
                    }
                }
            }
            if (cnt == 1) Grid[edge.posH][edge.posW] = false;
        }
    }
    private bool IsOutOfRange(Node node) => node.posH <= 0 || node.posH >= Height - 1 || node.posW <= 0 || node.posW >= Width - 1;

    public void Randomize()
    {
        for (int h = 0; h < Height; h += 1)
            for (int w = 0; w < Width; w += 1)
                Grid[h][w] = Random.Range(0.0f, 1.0f) > 0.5f;
    }
}
