using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminate.Interfaces.IlluminateQueue
{
    public interface IPriorityQueueEntity<K>: IComparable, ICloneable
    {
        K Key { get; }
    }
}
