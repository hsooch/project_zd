using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour
{
    private static Pool _i;
    private Dictionary<GameObject, Queue<GameObject>> _pools = new();

    private void Awake()
    {
        if (_i != null) { Destroy(gameObject); return; }
        _i = this; DontDestroyOnLoad(gameObject);
    }

    public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (_i == null)
        {
            var go = new GameObject("[Pool]");
            _i = go.AddComponent<Pool>();
            DontDestroyOnLoad(go);
        }
        if (!_i._pools.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            _i._pools[prefab] = q;
        }

        GameObject obj = null;
        while (q.Count > 0 && obj == null) obj = q.Dequeue();
        if (obj == null) obj = Instantiate(prefab);
        obj.transform.SetPositionAndRotation(pos, rot);
        return obj;
    }

    public static void Return(GameObject prefab, GameObject obj)
    {
        if (_i == null) return;
        obj.SetActive(false);
        if (!_i._pools.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            _i._pools[prefab] = q;
        }
        q.Enqueue(obj);
    }
}
