using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


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
        Action<RaycastHit> hitAction = NotifyHit;
        while (Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            pos.z = _camera.nearClipPlane;

            //Debug.DrawRay(_camera.transform.position, _camera.ScreenToWorldPoint(pos) * 10, Color.black);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit))
            {
                hitAction(_hit);
                hitAction = NotifyStay;
            }
            yield return null;
        }
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
}
