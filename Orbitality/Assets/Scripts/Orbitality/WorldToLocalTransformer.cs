using UnityEngine;

namespace Orbitality
{
    public class WorldToLocalTransformer
    {
        public WorldToLocalTransformer(Transform targetSpaceTransform) => _targetSpaceTransform = targetSpaceTransform;
        private Transform _targetSpaceTransform;
        public Vector3 Transform(Vector3 worldPoint) => _targetSpaceTransform.InverseTransformPoint(worldPoint);
    }
}