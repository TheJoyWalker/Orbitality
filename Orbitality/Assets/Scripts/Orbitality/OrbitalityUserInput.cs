using UnityEngine;

namespace Orbitality
{
    public class OrbitalityUserInput : IHitReceiver
    {
        private readonly FireController _fireController;
        public Planet Planet { get; set; }//could this change in future?
        private WorldToLocalTransformer _transformer;
        private Missile _missile;

        public OrbitalityUserInput(FireController fireController, WorldToLocalTransformer transformer, Planet planet)
        {
            _fireController = fireController;
            _transformer = transformer;
            Planet = planet;
        }
        public void OnPointerDown(Vector3 worldHitPoint)
        {
            //todo: player can shot without cooldown
            //if (Planet.CanShoot)
            _missile = _fireController.CreateMissile(Planet.PlanetData.WeaponType, Planet, GetAimPoint(worldHitPoint));
        }

        public void OnPointerStay(Vector3 worldHitPoint)
        {
            if (_missile == null)
                return;

            _fireController.AimAt(_missile, GetAimPoint(worldHitPoint));
        }

        private Vector3 GetAimPoint(Vector3 worldHitPoint) => _transformer.Transform(worldHitPoint);

        public void OnPointerUp(Vector3 worldHitPoint)
        {
            if (_missile == null)
                return;

            _fireController.Fire(_missile);
            _missile = null;
        }
    }
}