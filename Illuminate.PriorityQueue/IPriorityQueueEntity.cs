using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.PriorityQueue
{
    public interface IPriorityQueueEntity<K>: IComparable, ICloneable
    {
        K Key { get; }
    }
}
