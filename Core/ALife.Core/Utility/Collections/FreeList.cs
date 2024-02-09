using System;
using ALife.Core.CommonInterfaces;

namespace ALife.Core.Utility.Collections
{
    public class FreeList<T>
    {
        private SmallList<FreeElement> _data = null;

        private int _firstFree;

        public FreeList()
        {
        }

        public int Count { get; private set; }

        public int RangeMax => _data.Count;

        public int RangeMin => 0;

        public T this[int index]
        {
            get
            {
                if(_data == null)
                {
                    throw new InvalidOperationException("The list is empty.");
                }
                if(index < 0 || index >= _data.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be between 0 and {_data.Count - 1}.");
                }

                return _data[index].Element;
            }
            set
            {
                if(_data == null)
                {
                    throw new InvalidOperationException("The list is empty.");
                }
                if(index < 0 || index >= _data.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be between 0 and {_data.Count - 1}.");
                }

                _data[index].UpdateElement(value);
            }
        }

        public void Clear()
        {
            _data.Clear();
            _firstFree = -1;
            Count = 0;
        }

        public int Insert(T element)
        {
            Count++;
            if(_firstFree != -1)
            {
                int index = _firstFree;
                _firstFree = _data[index].Next;
                _data[index].UpdateElement(element);
                return index;
            }
            else
            {
                FreeElement freeElement = new FreeElement()
                {
                    Element = element,
                    Next = -1,
                };
                _data.PushBack(freeElement);
                return _data.Count - 1;
            }
        }

        public void RemoveAt(int index)
        {
            if(index < 0 || index >= _data.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be between 0 and {_data.Count - 1}.");
            }

            _data[index].UpdateNext(_firstFree);
            _firstFree = index;
        }

        public void Reserve(int size)
        {
            _data.Reserve(size);
        }

        public void Swap(FreeList<T> other)
        {
            int tempFirstFree = _firstFree;
            _data.Swap(other._data);
            _firstFree = other._firstFree;
            other._firstFree = tempFirstFree;
        }

        private struct FreeElement : IDeepCloneable<FreeElement>
        {
            public T Element;
            public int Next;

            public FreeElement Clone()
            {
                T newElement;
                if(Element is IDeepCloneable<T> cloneable)
                {
                    newElement = cloneable.Clone();
                }
                else
                {
                    newElement = Element;
                }

                return new FreeElement()
                {
                    Element = newElement,
                    Next = Next,
                };
            }

            public void Update(T element, int next)
            {
                Element = element;
                Next = next;
            }

            public void UpdateElement(T element)
            {
                Element = element;
            }

            public void UpdateNext(int next)
            {
                Next = next;
            }
        }
    }
}
