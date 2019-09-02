using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// this is a scratch of pointer hit resolver
/// </summary>
public class PointerHitResolver : MonoStaticUtility<PointerHitResolver>
{
    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() => InitUnityObject(true);


    private static readonly Dictionary<Collider, IHitReceiver> Receivers = new Dictionary<Collider, IHitReceiver>();
    public static void Subscribe(Collider collider, IHitReceiver receiver) => Receivers[collider] = receiver;
    public static bool Unsubscribe(Collider collider, IHitReceiver receiver) => Receivers.Remove(collider);

    [UsedImplicitly]
    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }

    private Coroutine _pointerDownCoroutine;
    //TODO: this way it suck, but I don't remember any other fast way to get it, consider it later
    private static Camera _camera;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_pointerDownCoroutine != null)
                StopCoroutine(_pointerDownCoroutine);
            _pointerDownCoroutine = StartCoroutine(PointerDownEnumerator());
        }
    }

    private IEnumerator PointerDownEnumerator()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            yield break;
        }

        Action<RaycastHit> hitAction = NotifyHit;
        while (Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            pos.z = _camera.nearClipPlane;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //TODO: consider supporting 2d colliders
            //RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            //if (hit2D.collider != null)
            //{
            //    hitAction(_hit);
            //    hitAction = NotifyStay;
            //}
            if (Physics.Raycast(ray, out _hit))
            {
                hitAction(_hit);
                hitAction = NotifyStay;
            }
            yield return null;
        }

        //TODO: consider using collection of hoovered objects
        if (_hit.collider != null)
            NotifyUp(_hit);
    }
    private static RaycastHit _hit;
    private static IHitReceiver _receiver;

    private void NotifyHit(RaycastHit hit)
    {
        if (Receivers.TryGetValue(hit.collider, out _receiver))
            _receiver.OnPointerDown(hit.point);
    }

    private void NotifyStay(RaycastHit hit)
    {
        if (Receivers.TryGetValue(hit.collider, out _receiver))
            _receiver.OnPointerStay(hit.point);
    }

    private void NotifyUp(RaycastHit hit)
    {
        if (Receivers.TryGetValue(hit.collider, out _receiver))
            _receiver.OnPointerUp(hit.point);
    }
}
