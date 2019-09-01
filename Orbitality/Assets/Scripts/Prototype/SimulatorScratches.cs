using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#pragma warning disable 649
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Prototype
{
    class SimulatorScratches
    {
        [Serializable]
        public struct PlanetData
        {
            public int Index;
            public float InitOrbitPosition;
            public float OrbitDistance;
            public float FullRotationTime;
            public float Mass;
            public float Radius;
            public float StepFactor => 1f / FullRotationTime;
        }

        [Serializable]
        public struct MissileData
        {
            public float Acceleration;
            public float Mass;
            public Vector2 Position;
        }

        public struct TrajectoryStep
        {
            public Vector2 Point;
            public PlanetData[] Planets;
        }

        public class OrbitalityModel
        {
            private PlanetData[] _emptyPlanetData;

            [SerializeField] private PlanetData[] _planetsData;
            [SerializeField] private List<MissileData> _missiles;

            private float _timePassed;
            public void Update(float time)
            {
                _timePassed += time;
            }

            public IEnumerable<Vector2> Positions => _planetsData.Select(x => GetPositionAtTime(x, _timePassed));

            public Vector2 GetPositionAtTime(PlanetData data, float time)
            {
                return new Vector2(Mathf.Cos(time + data.InitOrbitPosition), Mathf.Sin(time) + data.InitOrbitPosition) * data.FullRotationTime;
            }

            public TrajectoryStep[] GetTrajectory(MissileData data)
            {
                return GetTrajectoryPoints(data, _timePassed).ToArray();
            }

            private IEnumerable<TrajectoryStep> GetTrajectoryPoints(MissileData data, float time)
            {
                var hits = _emptyPlanetData;
                while (hits.Length == 0)
                {
                    var backUpTime = time;
                    var positions = _planetsData.Select(x => GetPositionAtTime(x, backUpTime));
                    var missilePosition = GetPosition(data);
                    time += Time.fixedDeltaTime;
                    hits = GetPlanetHits(data, time);
                    yield return new TrajectoryStep() { Point = missilePosition, Planets = hits };
                }
            }

            private Vector2 GetPosition(MissileData data)
            {
                throw new NotImplementedException();
            }

            private PlanetData[] GetPlanetHits(MissileData data, float time)
            {
                return _planetsData
                       .Where(x => SqrDistance(data.Position, GetPositionAtTime(x, time)) < x.Radius * x.Radius)
                       .ToArray();
            }

            private float SqrDistance(Vector2 one, Vector2 other)
            {
                var x = other.x - one.x;
                var y = other.y - one.y;
                return x * x + y * y;
            }
        }
    }
}
