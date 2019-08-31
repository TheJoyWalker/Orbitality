using System;
using System.Collections;
using UnityEngine;

namespace Orbitality
{
    public class Missile : MonoBehaviour, ICollision2DAgent
    {
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private Collision2DAgent _collision2DAgent;
        private Coroutine _dieInTime;
        [SerializeField] private float _forwardAcceleration;
        public bool _isAccelerating;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        public int Id; //TODO: resolve id properly
        public Planet Owner { get; set; }

        public event EventHandler<Collision2D> Entered;
        public event EventHandler<Collider2D> Staying;
        public event EventHandler<Collision2D> Exited;
        public event EventHandler Died;

        private void Awake()
        {
            _collision2DAgent.Entered += Collision2DAgentOnEntered;
            _collision2DAgent.Staying += Collision2DAgentOnStaying;
            _collision2DAgent.Exited += Collision2DAgentOnExited;
        }


        public void SpawnReset()
        {
            CancelTimedDeath();
            Stop();
        }

        internal void Die() => OnDied();

        public void Die(float time)
        {
            CancelTimedDeath();
            StartCoroutine(DieInTimeCoroutine(time));
        }

        private void CancelTimedDeath()
        {
            if (_dieInTime != null)
            {
                StopCoroutine(_dieInTime);
                _dieInTime = null;
            }
        }

        private IEnumerator DieInTimeCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            Die();
        }

        public void Stop()
        {
            _isAccelerating = false;
            _collider2D.enabled = false;
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
        }

        public void Fire()
        {
            _isAccelerating = true;
            _collider2D.enabled = true;
        }

        public void FixedUpdate()
        {
            if (_isAccelerating)
                _rigidbody2D.velocity = _rigidbody2D.velocity + (Vector2)transform.up * _rigidbody2D.mass *
                                        _forwardAcceleration * Time.fixedDeltaTime;
        }

        public void LookAt(Vector2 point)
        {
            transform.localRotation = Quaternion.FromToRotation(Vector2.up, point - (Vector2)transform.localPosition);
        }


        private void Collision2DAgentOnExited(object sender, Collision2D e) => Exited?.Invoke(this, e);
        private void Collision2DAgentOnStaying(object sender, Collider2D e) => Staying?.Invoke(this, e);
        private void Collision2DAgentOnEntered(object sender, Collision2D e) => Entered?.Invoke(this, e);

        protected virtual void OnDied() => Died?.Invoke(this, EventArgs.Empty);
    }
}