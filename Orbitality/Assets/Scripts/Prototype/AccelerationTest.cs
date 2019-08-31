using UnityEngine;

public class AccelerationTest : MonoBehaviour
{
    [SerializeField] private float _acceleration;
    //[SerializeField] private Vector2 _force;
    [SerializeField] private float _mass = 1;
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private float _drag = 0;


    /*
    linearVelocity *= 1.0f / (1.0f + timeSlice* LinearDrag)
    angularVelocty *= 1.0f / (1.0f + timeSlice* AngularDrag)

    For completeness, the body linear velocity is updated like so:
    linearVelocity += timeSlice* (GravityScale* WorldGravity + (1.0f / Mass) * BodyAppliedForce)

    Here's how the angular velocity is updated
        angularVelocity += timeSlice* (1.0f / Inertia) * BodyAppliedTorque
    */
    private void FixedUpdate()
    {
        var frameVelocity = Vector2.up * _acceleration * _mass * Time.deltaTime;
        _velocity += frameVelocity;
        _velocity *= Mathf.Clamp01(1f - _drag * Time.deltaTime);


        var displacement = _velocity * Time.deltaTime;
        transform.position = transform.position + (Vector3)displacement;

        Debug.Log($"[{gameObject.name}] v = {_velocity}, p = {transform.position.y}" +
                  $"\nposition: {transform.position} displacement: {displacement}");


        //Vector3 newVelocity = rb.velocity + gForceVector * rb.mass * Time.deltaTime;

        //_velocity = _velocity + Vector2.up * _acceleration * _mass * Time.deltaTime;
        //_velocity = _velocity + _acceleration * Time.fixedDeltaTime;
        //transform.position = (Vector2) transform.position + _velocity * Time.deltaTime;
    }
}