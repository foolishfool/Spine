using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomOneNoRepeat
{
    public int Count;
    public List<int> list = new List<int>();
    public delegate void EndHandler();
    public EndHandler OnEnd;
    public RandomOneNoRepeat(int Count)
    {
        this.Count = Count;
        list = new List<int>();
        Revert();
    }
    public RandomOneNoRepeat(int Count,EndHandler OnEnd)
    {
        this.Count = Count;
        this.OnEnd = OnEnd;
        list = new List<int>();
        Revert();
    }

    public void Revert()
    {
        for (int i = 0; i < Count; i++)
            list.Add(i);
    }

    public int RandomOne()
    {
        if (Count == 0)
            return 0;
        if (list.Count == 0)
            Revert();
        int index = Random.Range(0, list.Count);
        int result = list[index];
        list.RemoveAt(index);
        if (list.Count == 0 && OnEnd != null)
            OnEnd();
        return result;
    }

    public int RandomOne(EndHandler OnEnd)
    {
        this.OnEnd = OnEnd;
        return RandomOne();
    }
}
