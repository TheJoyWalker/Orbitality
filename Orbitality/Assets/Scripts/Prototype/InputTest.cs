﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#pragma warning disable 649
// ReSharper disable FieldCanBeMadeReadOnly.Local
public class InputTest : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private MissileLauncherTest _missileLauncherTest;
    [SerializeField] private Transform _hitCursor;
    private Coroutine _aimingCoroutine;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_aimingCoroutine != null)
                StopCoroutine(_aimingCoroutine);
            _aimingCoroutine = StartCoroutine(Aim());
        }



        //Plane p;


        //if (Input.GetMouseButtonDown(0))
        //{
        //    _missileLauncher.CreateMissile();
        //    return;
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    _missileLauncher.FireMissile();
        //    return;
        //}
    }

    private IEnumerator Aim()
    {
        _missileLauncherTest.CreateMissile();
        RaycastHit hit;
        while (Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            //Debug.Log($"mpos: {pos}");
            pos.z = _camera.nearClipPlane;
            //_missileLauncher.AimAt(_camera.ScreenToWorldPoint(pos));

            Debug.DrawRay(_camera.transform.position, _camera.ScreenToWorldPoint(pos) * 10, Color.black);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Debug.Log($"hit: {pos}");
                _hitCursor.position = hit.point;
                _missileLauncherTest.AimAt(hit.point);
            }
            yield return null;
        }
        _missileLauncherTest.FireMissile();
    }
}
