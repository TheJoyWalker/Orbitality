using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueTester : MonoBehaviour
{
    [SerializeField] private AccelerationTest _accelerationTest;
    [SerializeField] private Rigidbody2D _rigidbody2D;

    void FixedUpdate()
    {
        _rigidbody2D.AddTorque(1, ForceMode2D.Force);
    }
}
