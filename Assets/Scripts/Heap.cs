using System.Collections;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    /*
     * Removes the first item from the heap (the item with the highest priority).
     * Then sorts out the rest of the heap to fill the gap.
     */
    public T RemoveFirst()
    {
        T firstItem = items[0];

        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get { return currentItemCount; }
    }

    /*
     * Checks if the items array contains the passed item in the index 
     * position indicated by that item's heapIndex.
     */
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    /*
     * Used to sort heap after removing an element
     */
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // Checks to see if we have reached the bottom of the heap
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                // Checks if there is a second child
                if (childIndexRight < currentItemCount)
                {
                    // If right child has higher priority, swap with right child
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                // Swap item with child with current swapIndex if it has higher priority
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                    return;
            }
            else
                return;
        }
    }

    /*
     * Used to sort heap after adding an element
     */
    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
                break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
