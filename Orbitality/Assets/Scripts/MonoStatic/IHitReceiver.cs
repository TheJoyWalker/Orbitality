using UnityEngine;

public interface IHitReceiver
{
    void OnPointerDown(Vector3 worldHitPoint);
    void OnPointerStay(Vector3 worldHitPoint);
    void OnPointerUp(Vector3 worldHitPoint);
}