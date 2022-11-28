using System.Collections;
using System.Collections.Generic;

public class ListUtility
{
    /// <summary>
    /// Create a 2D List with type T
    /// </summary>
    public static List<List<T>> List2D<T>(int height, int width, T defaultValue)
    {
        List<List<T>> newList = new();

        for (int h = 0; h < height; h += 1)
        {
            List<T> newRow = new();
            for (int w = 0; w < width; w += 1)
                newRow.Add(defaultValue);
            newList.Add(newRow);
        }

        return newList;
    }
}
