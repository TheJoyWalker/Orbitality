using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Pools
{
    public interface IItemCollection<out T> : IEnumerable<T>
    {

    }
    public class SimpleMonoPool<T> : IItemCollection<T> where T : Object
    {
        private readonly Queue<T> _free = new Queue<T>();
        public readonly IList<T> _busy = new List<T>();
        private readonly T _prefab;

        public T Spawn() => _free.Count > 0 ? _free.Dequeue() : Object.Instantiate(_prefab);

        public void Release(T item)
        {
            if (item.GetType() != typeof(T))
                throw new InvalidEnumArgumentException();
            _free.Enqueue(item);
        }

        public IEnumerator<T> GetEnumerator() => _busy.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}