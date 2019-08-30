using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private float _distance = 1;
    [SerializeField] private float _rotationStep;

    void FixedUpdate()
    {
        _rotationStep += _speed;
        transform.localPosition = new Vector2(_distance * Mathf.Sin(_rotationStep), _distance * Mathf.Cos(_rotationStep));
    }
}
