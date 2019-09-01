using Assets.Scripts.Storages;
using Orbitality;
using UnityEngine;
#pragma warning disable 649
// ReSharper disable FieldCanBeMadeReadOnly.Local
public class WorldSaveButtonTest : MonoBehaviour, IHitReceiver
{
    public Transform[] _pausedTransforms;
    [SerializeField] private Collider _collider;
    void OnEnable()
    {
        PointerHitResolver.Subscribe(_collider, this);
    }

    void OnDisable()
    {
        PointerHitResolver.Unsubscribe(_collider, this);
    }

    public void OnPointerDown(Vector3 worldHitPoint)
    {
        var sm = new SaveManager<PlanetData>();
        Debug.Log($"save list = [{string.Join(",", sm.GetSaves())}]");
    }

    public void OnPointerStay(Vector3 worldHitPoint)
    {

    }

    public void OnPointerUp(Vector3 worldHitPoint)
    {

    }
}
