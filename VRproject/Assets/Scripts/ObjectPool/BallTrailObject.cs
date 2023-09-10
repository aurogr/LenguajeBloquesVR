using UnityEngine;

public class BallTrailObject : RecyclableObject
{
    private void OnEnable()
    {
        GameManager.Instance.OnSceneReset += Recycle; // when the scene is reseted (GameManager event), recycle the object
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSceneReset -= Recycle; // desuscribe when the object is disabled
    }

}
