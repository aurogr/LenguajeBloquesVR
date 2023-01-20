using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _repositionTransform;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (!collision.gameObject.CompareTag("Container"))
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.transform.position = _repositionTransform.position;
        }
    }
}
