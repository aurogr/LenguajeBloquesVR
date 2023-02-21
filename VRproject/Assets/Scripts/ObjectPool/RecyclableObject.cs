using UnityEngine;

public abstract class RecyclableObject : MonoBehaviour
{
    private ObjectPool _belongingObjectPool;

    public void Configure(ObjectPool objectPool) // this is called automatically from the object pool when it creates the object
                                                 // to correctly assign their pool to each object so they can use it
    {
        _belongingObjectPool = objectPool;
    }

    public void Recycle() // this needs to be called from the correspondant script
                          // when we want to stop using this object and return it to the pool
    {
        _belongingObjectPool.RecycleObject(this);
    }
}
