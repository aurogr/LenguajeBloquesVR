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

    /// <summary>
    /// Tell this puzzle piece to tell its socket that it is now inside a block
    /// </summary>
    /// <param name="fatherLoop" >Give the father block so that it stores a reference to it</param>
    public void SetFatherBlockPointer(BlockBehaviour fatherLoop)
    {
        _socket.SetFatherBlockPointer(fatherLoop);
    }

    /// <summary>
    /// Tell this puzzle piece to tell its socket that it is no longer inside a block and should stop pointing to it
    /// </summary>
    public void RemoveFatherBlockPointer()
    {
        _socket.RemoveFatherBlockPointer();
    }
    #endregion
}

public enum PuzzlePieceType { right, left, up, down, forLoop, conditional, condition};

