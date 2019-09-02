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
    public class SimpleMonoPool<T> where T : MonoBehaviour
    {
        public SimpleMonoPool(T prefab) => _prefab = prefab;

        private readonly Queue<T> _free = new Queue<T>();
        public readonly IList<T> Busy = new List<T>();//todo: should not give direct access

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

        private T GetFreeOrNew()
        {
            //todo: temp hack, investigate
            return Object.Instantiate(_prefab);
            //var item = _free.Count > 0 ? _free.Dequeue() : Object.Instantiate(_prefab);
            //Busy.Add(item);
            //return item;
        }

        public void Release(T item)
        {
            //todo: temp hack, investigate
            if (item != null)
                Object.Destroy(item.gameObject);
            //if (item == null)
            //    throw new ArgumentException("You can not release a null item");
            //if (item.GetType() != typeof(T))
            //    throw new InvalidEnumArgumentException();
            //item.gameObject.SetActive(false);
            //_free.Enqueue(item);
            //Busy.Remove(item);
        }
    }
}