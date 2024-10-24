using System;
using System.Collections.Generic;

public class MinHeap<T> where T : IComparable<T>
{
    private List<T> _items;

    public MinHeap()
    {
        _items = new List<T>() { default }; // 1-based index
    }

    public void Push(T item)
    {
        int index = _items.Count;
        _items.Add(item);

        // heapify
        while (index > 1)
        {
            int parent = index / 2;

            if (_items[index].CompareTo(_items[parent]) >= 0)
            {
                break;
            }

			Swap(index, parent);
			index = parent;
		}
    }

    public T Pop()
    {
        T item = _items[1];

        int lastIndex = _items.Count - 1;
        _items[1] = _items[lastIndex];
        _items.RemoveAt(lastIndex);

        // heapify
        int index = 1;
        while (true)
        {
            int left = index << 1;
            int right = (index << 1) + 1;
            int smallest = index;

            // find smaller value
            if (left < _items.Count && _items[left].CompareTo(_items[smallest]) < 0)
            {
                smallest = left;
            }
            if (right < _items.Count && _items[right].CompareTo(_items[smallest]) < 0)
            {
                smallest = right;
            }

            if (smallest == index)
            {
                break;
            }

            Swap(index, smallest);
            index = smallest;
        }

        return item;
    }

    public bool IsEmpty => _items.Count <= 1;

    public T Top()
    {
        return _items[1];
    }

    private void Swap(int i1, int i2)
    {
        T temp = _items[i1];
        _items[i1] = _items[i2];
        _items[i2] = temp;
    }

    public int Count => _items.Count - 1;

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }
}
