using UnityEngine;

public class ObjectPoolSpawner : MonoBehaviour
{
    [SerializeField] RecyclableObject _prefab;
    [SerializeField] int _objectPoolSize;
    ObjectPool _objectPool;

    private void Awake()
    {
        InitializePool();
        SpawnObject();
    }

    void InitializePool()
    {
        _objectPool = new ObjectPool(_prefab, transform);
        _objectPool.Init(_objectPoolSize);
    }

    public RecyclableObject SpawnObject() // should be called when we want to spawn a puzzle piece
    {
        return _objectPool.SpawnRecycledObject();
    }
}
