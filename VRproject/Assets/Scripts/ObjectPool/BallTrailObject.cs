using UnityEngine;

public class BallTrailObject : RecyclableObject
{
    public void SetTrailPositionAndMaterial(Transform place, Material material)
    {
        transform.position = place.position;
        transform.rotation = place.rotation;
        gameObject.GetComponent<Renderer>().material = material;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnSceneReset += Recycle; // when the scene is reseted (GameManager event), recycle the object
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSceneReset -= Recycle; // desuscribe when the object is disabled
    }

}
