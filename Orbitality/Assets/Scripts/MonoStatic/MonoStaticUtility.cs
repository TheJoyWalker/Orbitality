using UnityEngine;


public class MonoStaticUtility<T> : MonoBehaviour where T : MonoBehaviour
{
    /*
     Target Class should have 
     [RuntimeInitializeOnLoadMethod]
     public static void Initialize() => InitUnityObject(true);

     if Order to maintain instances
     */

    protected static T Instance;
    protected static void InitUnityObject(bool visible = false)
    {
        if (Instance != null)
            return;
        var go = new GameObject(typeof(T).Name);
        DontDestroyOnLoad(go);

        if (!visible)
            go.hideFlags = HideFlags.HideAndDontSave;
        Instance = go.AddComponent<T>();
    }

    protected virtual void Awake()
    {
        var thisInstance = this as T;

        if (Instance == null)
        {
            Instance = thisInstance;
        }
        else
        {
            DestroyImmediate(this);
        }

    }

    protected void OnDisable()
    {
        var thisInstance = this as T;
        if (Instance == thisInstance)
            Instance = null;
    }
}