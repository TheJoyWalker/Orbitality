using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pools
{
    public interface IItemCollection<out T> : IEnumerable<T>
    {

    }
    public class SimpleMonoPool<T> : IItemCollection<T> where T : MonoBehaviour
    {
        public SimpleMonoPool(T prefab) => _prefab = prefab;

        private readonly Queue<T> _free = new Queue<T>();
        public readonly IList<T> _busy = new List<T>();
        private readonly T _prefab;

        public T Spawn()
        {
            var obj = GetFreeOrNew();
            obj.gameObject.SetActive(true);
            return obj;
        }
        public T Spawn(Action<T> prepareAction)
        {
            var obj = GetFreeOrNew();
            prepareAction(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }

        private T GetFreeOrNew() => _free.Count > 0 ? _free.Dequeue() : Object.Instantiate(_prefab);

        public void Release(T item)
        {
            if (item.GetType() != typeof(T))
                throw new InvalidEnumArgumentException();
            item.gameObject.SetActive(false);
            _free.Enqueue(item);
        }

        public IEnumerator<T> GetEnumerator() => _busy.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}