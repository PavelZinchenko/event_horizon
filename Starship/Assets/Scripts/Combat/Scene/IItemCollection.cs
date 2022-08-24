using System;
using System.Collections.Generic;

namespace Combat.Scene
{
    public interface IUnitList<T> where T : IDisposable
    {
        IList<T> Items { get; }
        object LockObject { get; }
    }
}
