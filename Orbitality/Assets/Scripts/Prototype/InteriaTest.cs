using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 649
// ReSharper disable FieldCanBeMadeReadOnly.Local

public class InteriaTest : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody2D;

    void FixedUpdate()
    {
        Debug.Log($"inertia = {_rigidbody2D.inertia}");
    }
}
