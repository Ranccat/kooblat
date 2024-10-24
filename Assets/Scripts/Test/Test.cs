using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    HashSet<int> set;

    private void Start()
    {
        set = new();
        for (int i = 0; i < 5; i++)
            new Nig(set);
        foreach (int i in set)
            print(i);
    }

    class Nig
    {
        static int cnt;

        public Nig(HashSet<int> set)
        {
            set.Add(cnt++);
        }
    }

}
