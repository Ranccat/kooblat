using UnityEngine;
using System.Collections.Generic;

public static class RandomUtils
{
    // Fisher-yates shuffle
    public static void Shuffle<T>(ref T[] arr)
    {
        for (int end = arr.Length - 1; end > 0; end--)
        {
            int randIndex = Random.Range(0, end - 1);
            T temp = arr[randIndex];
            arr[randIndex] = arr[end];
            arr[end] = temp;
        }
    }

    public static void Shuffle<T>(ref List<T> list)
    {
        for (int end = list.Count - 1; end > 0; end--)
        {
            int randIndex = Random.Range(0, end - 1);
            T temp = list[randIndex];
            list[randIndex] = list[end];
            list[end] = temp;
        }
    }

    public static bool Test(float chance)
    {
        return Random.Range(0f, 1f) <= chance;
    }
}
