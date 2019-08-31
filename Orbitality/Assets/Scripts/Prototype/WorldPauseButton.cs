using UnityEngine;

public class WorldPauseButton : MonoBehaviour, IHitReceiver
{
    public Transform[] _pausedTransforms;
    [SerializeField] private Collider _collider;
    void OnEnable()
    {
        PauseManager.PauseChanged += PauseManagerOnPauseChanged;
        PointerHitResolver.Subscribe(_collider,this);
    }

    void OnDisable()
    {
        PauseManager.PauseChanged -= PauseManagerOnPauseChanged;
    }

    private void PauseManagerOnPauseChanged(object sender, bool e)
    {
        foreach (var pausedTransform in _pausedTransforms)
        {
            pausedTransform.gameObject.SetActive(e);
        }
    }

    public void OnPointerDown(Vector3 worldHitPoint) => PauseManager.Paused = !PauseManager.Paused;

    public void OnPointerStay(Vector3 worldHitPoint)
    {
        
    }
}
