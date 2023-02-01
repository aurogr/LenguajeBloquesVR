using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] Transform _repositionTransform;
    [SerializeField] AudioClip _collisionAudioClip;
    [SerializeField] PuzzlePieceType _pieceType = PuzzlePieceType.right;

    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Container"))
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.transform.position = _repositionTransform.position;
        }

        // _audioSource.PlayOneShot(_collisionAudioClip); // to play audio on coliision, deactivated temporarily but should be on final version
    }

    public PuzzlePieceType GetPieceType()
    {
        return _pieceType;
    }
}

public enum PuzzlePieceType { right, left, up, down};

