using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.PriorityQueue
{
    public class LinkedListEntity<T>
    {
        protected LinkedListEntity<T> _nextEntity;
        protected T _obj;
        protected bool _isRemoved;

        public LinkedListEntity<T> Next
        {
            get { return _nextEntity; }
            set { _nextEntity = value; }
        }

        public T Value
        {
            get { return _obj; }
        }

        public LinkedListEntity(T value)
        {
            _obj = value;
        }

        public bool IsRemoved
        {
            get { return _isRemoved; }
            set { _isRemoved = value; }
        }
                 

    }
}
