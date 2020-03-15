using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBuffer<T>
{
    public RingBuffer(T[] givenValues)
    {
        values = givenValues;
    }

    public T getNextValue() 
    {
        int index = currentIndex;
        currentIndex++;
        currentIndex = currentIndex % values.Length;
        return values[index];
    }

    private int currentIndex = 0;
    private T[] values = null;
}
