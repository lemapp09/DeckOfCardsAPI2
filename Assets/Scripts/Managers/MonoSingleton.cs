using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance {
        get {
            if(_instance == null) {
                Debug.Log(typeof(T).ToString() + " is NULL");
            }
            return _instance;
        }
    }

    public void Awake() {
        _instance = this as T;
        // Cast as T
        // _instance = (T)this;
    }
}
