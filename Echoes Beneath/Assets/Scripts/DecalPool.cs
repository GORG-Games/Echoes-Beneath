using UnityEngine;
using System.Collections.Generic;

public class DecalPool : MonoBehaviour
{
    public static DecalPool Instance { get; private set; }

    [System.Serializable]
    public class DecalEntry
    {
        public GameObject prefab;
        public int initialCount = 10;
    }

    [SerializeField] private List<DecalEntry> decalTypes;

    private Dictionary<string, Queue<GameObject>> _pools = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var entry in decalTypes)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < entry.initialCount; i++)
            {
                GameObject obj = Instantiate(entry.prefab, transform);
                obj.name = entry.prefab.name; // для соответствия ключу
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            _pools.Add(entry.prefab.name, queue);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!_pools.TryGetValue(prefab.name, out var pool))
        {
            Debug.LogWarning($"[DecalPool] No pool found for {prefab.name}. Instantiating.");
            return Instantiate(prefab, position, rotation);
        }

        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(transform); // можно убрать, если не хочешь хранить под DecalPool
        obj.SetActive(true);
        return obj;
    }
}