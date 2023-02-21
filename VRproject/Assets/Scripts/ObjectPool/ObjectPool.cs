using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    RecyclableObject _prefab;
    Queue<RecyclableObject> _recycledObjects;
    Transform _container;

    #region Initialize pool and objects
    public ObjectPool(RecyclableObject prefab, Transform container)
    {
        _prefab = prefab;
        _container = container;
    }

    public void Init(int poolSize)
    {
        _recycledObjects = new Queue<RecyclableObject>();

        for (int i = 0; i < poolSize; i++)
        {
            _recycledObjects.Enqueue(InstantiateNewObject());
        }
    }

    private RecyclableObject InstantiateNewObject()
    {
        RecyclableObject objectInstance = GameObject.Instantiate(_prefab, _container);
        objectInstance.transform.localPosition = Vector3.zero;
        objectInstance.gameObject.SetActive(false);
        objectInstance.Configure(this);
        return objectInstance;
    }
    #endregion

    #region Use pool
    public void RecycleObject(RecyclableObject objectToRecycle)
    {
        objectToRecycle.gameObject.SetActive(false);
        objectToRecycle.transform.SetParent(_container);
        objectToRecycle.transform.localPosition = Vector3.zero;
        _recycledObjects.Enqueue(objectToRecycle);
    }

    public void SpawnRecycledObject()
    {
        if (_recycledObjects.Count > 0)
        {
            RecyclableObject recycledObject = _recycledObjects.Dequeue();
            recycledObject.transform.SetParent(null);
            recycledObject.gameObject.SetActive(true);
        }
        else
        {
            // instantiate a new one in demand
            RecyclableObject instantiatedObject = InstantiateNewObject();
            instantiatedObject.transform.SetParent(null);
            instantiatedObject.gameObject.SetActive(true);
            Debug.Log("You already have a lot of pieces, I had to instantiate a new one for you");
        }
    }

    #endregion
}
