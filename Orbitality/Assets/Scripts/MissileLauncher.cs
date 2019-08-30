using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    [SerializeField] private Transform _missile;
    [SerializeField] private CircleCollider2D _circleCollider2D;

    private Transform _currentMissile;
    public void CreateMissile()
    {
        _currentMissile = Instantiate(_missile, Vector3.zero, Quaternion.identity, transform);
    }

    public void FireMissile()
    {

    }

    public void AimAt(Vector3 worldPoint)
    {
        Debug.Log("up" + worldPoint);
        if (_currentMissile == null)
            return;

        var localPointerPosition = transform.InverseTransformPoint(worldPoint);
        var targetPosition = localPointerPosition.normalized * _circleCollider2D.radius;
        _currentMissile.localPosition = targetPosition;
        _currentMissile.up = (worldPoint - _currentMissile.position).normalized;
        UnityEditor.Selection.activeGameObject = _currentMissile.gameObject;


    }
}