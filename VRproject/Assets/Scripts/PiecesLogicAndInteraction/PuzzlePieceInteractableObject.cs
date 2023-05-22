using UnityEngine;

public class PuzzlePieceInteractableObject : RecyclableObject
{
    Transform _repositionTransform;
    [SerializeField] AudioClip _collisionAudioClip;
    [SerializeField] PuzzlePieceType _pieceType = PuzzlePieceType.right;
    [SerializeField] GameConditions _conditionType = GameConditions.None;

    CustomSocketInteractor _socket;
    AudioSource _audioSource;

    private void Awake()
    {
        _repositionTransform = GameObject.FindGameObjectWithTag("Reposition").transform;
        _audioSource = gameObject.GetComponent<AudioSource>();
        _socket = GetComponentInChildren<CustomSocketInteractor>();
        //_socket.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneReset += ResetPiece;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneReset -= ResetPiece;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OutsideBounds")) // if the object is unreacheable to the player return it back to the table
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.transform.position = _repositionTransform.position;
        }

        // _audioSource.PlayOneShot(_collisionAudioClip); // to play audio on collision, deactivated temporarily but should be on final version
    }

    private void ResetPiece()
    {
        Recycle(); // recycable object behaviour
    }
    

    public void ActivateSocket()
    {
        _socket.ActivateSocket();
    }

    public void DeactivateSocket()
    {
        _socket.DeactivateSocket();
    }

    #region Getters / setters etc

    public PuzzlePieceType GetPieceType()
    {
        return _pieceType;
    }
    
    public GameConditions GetConditionType()
    {
        return _conditionType;
    }

    public void SetFatherLoop(BlockBehaviour fatherLoop)
    {
        _socket.SetFatherLoop(fatherLoop);
    }

    public void RemoveFatherLoop()
    {
        _socket.RemoveFatherLoop();
    }
    #endregion
}

public enum PuzzlePieceType { right, left, up, down, forLoop, conditional, condition};

