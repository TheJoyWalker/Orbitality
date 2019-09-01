using System.Collections.Generic;
using Orbitality;
using Pools;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PlanetBarManager : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] private PlanetBar _planetBarPrefab;

        private readonly List<PlanetBar> _planetBars = new List<PlanetBar>();
        public SimpleMonoPool<PlanetBar> PlanetBarPool;

        void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _planetBarPrefab.gameObject.SetActive(false);
            PlanetBarPool = new SimpleMonoPool<PlanetBar>(_planetBarPrefab);
        }

        public void Add(Planet planet)
        {
            if (PlanetBarPool == null)
                Initialize();

            var bar = PlanetBarPool.Spawn(x =>
                                          {
                                              x._canvas = _canvas;
                                              x.transform.SetParent(transform);
                                              x.Planet = planet;
                                              x.transform.localScale = Vector3.one;
                                          });
            _planetBars.Add(bar);
        }

        public void AddRange(IEnumerable<Planet> planets)
        {
            foreach (var planet in planets)
            {
                Add(planet);
            }
        }

        public bool Remove(Planet planet)
        {
            var idx = _planetBars.FindIndex(x => x.Planet == planet);
            if (idx == -1)
                return false;

            PlanetBarPool.Release(_planetBars[idx]);
            _planetBars.RemoveAt(idx);
            return true;
        }

        public void RemoveRange(List<Planet> planets)
        {
            foreach (var planet in planets)
            {
                Remove(planet);
            }
        }
    }
}