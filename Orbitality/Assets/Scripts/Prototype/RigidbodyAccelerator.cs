using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 649
// ReSharper disable FieldCanBeMadeReadOnly.Local

public class RigidbodyAccelerator : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 gForceVector = Vector2.up * -9.8f;
    [SerializeField] private float _acceleration = -9.8f;
    [SerializeField] private float _drag;

    void FixedUpdate()
    {
        Vector3 newVelocity = rb.velocity + Vector2.up * _acceleration * rb.mass * Time.deltaTime;
        //Vector3 newVelocity = rb.velocity + gForceVector * rb.mass * Time.deltaTime;
        newVelocity = newVelocity * Mathf.Clamp01(1f - _drag * Time.deltaTime);
        rb.velocity = newVelocity;
        Debug.Log($"[{gameObject.name}]v = {rb.velocity}, p = {transform.position.y}");
    }
}
