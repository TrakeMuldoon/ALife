using System;
using ALife.Core.CommonInterfaces;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Utility.Collections
{
    /// <summary>
    /// A list that is optimized for small numbers of elements.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SmallList<T>
    {
        /// <summary>
        /// The default capacity
        /// </summary>
        public const int DEFAULT_CAPACITY = 2;

        /// <summary>
        /// The fixed cap
        /// </summary>
        public const int FIXED_CAP = 256;

        /// <summary>
        /// The buffer
        /// </summary>
        private T[] _buffer = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmallList{T}"/> class.
        /// </summary>
        public SmallList()
        {
            Capacity = 0;
            Count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmallList{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity to reserve.</param>
        public SmallList(int capacity)
        {
            if(capacity > FIXED_CAP || capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} must be between 0 and {FIXED_CAP}.");
            }

            Reserve(capacity);
            Count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmallList{T}"/> class by cloning an existing list.
        /// </summary>
        /// <param name="other">The other list to clone.</param>
        public SmallList(SmallList<T> other)
        {
            if(other.Count == 0)
            {
                return;
            }

            Reserve(other.Count);
            for(int i = 0; i < other.Count; i++)
            {
                T clonedItem;
                bool _cloneable = typeof(T).GetInterface(nameof(IDeepCloneable<T>)) != null;
                if(_cloneable)
                {
                    clonedItem = other._buffer[i].Clone();
                }
                else
                {
                    clonedItem = other._buffer[i];
                }
                _buffer[i] = clonedItem;
            }
            Count = other.Count;
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="T"/>.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if(index < 0 || index >= FIXED_CAP || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be between 0 and {Count}.");
                }
                return _buffer[index];
            }
            set
            {
                if(index < 0 || index >= FIXED_CAP || index >= Capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be between 0 and {Capacity}.");
                }
                _buffer[index] = value;
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Count = 0;
            Capacity = 0;
            _buffer = null;
        }

        /// <summary>
        /// Returns an index to a matching element in the list or -1 if the element is not found.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The index if the element is found, or -1 if it is not.</returns>
        public int FindIndex(T element)
        {
            for(var i = 0; i < Count; i++)
            {
                if(_buffer[i].Equals(element))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Pops an element from the back of the list.
        /// </summary>
        /// <returns>The popped instance.</returns>
        public T PopBack()
        {
            return _buffer[--Count];
        }

        /// <summary>
        /// Inserts the specified element to the back of the list.
        /// </summary>
        /// <param name="element">The element.</param>
        public void PushBack(T element)
        {
            if(Capacity == 0)
            {
                Reserve(DEFAULT_CAPACITY);
            }
            else if(Count == Capacity)
            {
                Reserve(Capacity * 2);
            }
            _buffer[Count++] = element;
        }

        /// <summary>
        /// Reserves space for the specified number of elements.
        /// </summary>
        /// <param name="size">The size.</param>
        public void Reserve(int size)
        {
            int actualSize = ExtraMath.Clamp(size, 0, FIXED_CAP);
            if(actualSize == Capacity)
            {
                return;
            }
            if(actualSize == 0)
            {
                Clear();
                return;
            }
            if(actualSize < Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"{nameof(size)} must be greater than or equal to the current capacity {Capacity}.");
            }

            T[] newBuffer = new T[actualSize];
            for(int i = 0; i < Count; i++)
            {
                newBuffer[i] = _buffer[i];
            }

            _buffer = newBuffer;
            Capacity = actualSize;
        }

        /// <summary>
        /// Swaps the contents of this list with another list.
        /// </summary>
        /// <param name="other">The other.</param>
        public void Swap(SmallList<T> other)
        {
            T[] myBuffer = _buffer;
            int myCount = Count;
            int myCapacity = Capacity;

            _buffer = other._buffer;
            Count = other.Count;
            Capacity = other.Capacity;

            other._buffer = myBuffer;
            other.Count = myCount;
            other.Capacity = myCapacity;
        }
    }
}
