using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDebug : MonoBehaviour
{
    public static void OutputMatrix(int[,] matrix)
    {
        List<string> strings = new List<string>();

        for (int y = 0;  y < matrix.GetLength(1); y++)
        {
            string newLine = "\n ";

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                newLine = newLine + " [" + matrix[x, y] +"]";
            }

            strings.Add(newLine);
        }

        string finalString = string.Empty;

        foreach (string individualString in  strings)
        {
            finalString += individualString;
        }

        Debug.Log(finalString);
    }
}
