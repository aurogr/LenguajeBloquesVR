using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzlePieceInteractableObject : RecyclableObject
{
    Transform _repositionTransform;
    [SerializeField] AudioClip _collisionAudioClip;
    [SerializeField] PuzzlePieceType _pieceType = PuzzlePieceType.right;
    
    int _times = 1;
    TMP_Dropdown _dropdown;

    CustomSocketInteractor _socket;
    AudioSource _audioSource;

    public override void Recycle()
    {
        if (_pieceType == PuzzlePieceType.forLoop)
        {
            GetComponentInChildren<BlockBehaviour>().ResetSize();
        }

        if(_dropdown is not null)
        {
            _dropdown.value = 0;
            _times = 1;
        }

        base.Recycle();
    }

    private void Awake()
    {
        _repositionTransform = GameObject.FindGameObjectWithTag("Reposition").transform;
        _audioSource = gameObject.GetComponent<AudioSource>();
        _socket = GetComponentInChildren<CustomSocketInteractor>();

        _dropdown = GetComponentInChildren<TMP_Dropdown>();
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
        if (!GameManager.Instance.GetIsGameSituationTheSame()) // Piece only resets if the game situation changes, if the player is still trying the same screen pieces stay
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

    /// <summary>
    /// Get number of times that the piece inside this socket will be executed
    /// </summary>
    public int GetTimes()
    {
        return _times;
    }

    /// <summary>
    /// Set number of times that the piece inside this socket will be executed
    /// </summary>
    public void SetTimes(int times)
    {
        _times = times + 1; // because the times are chosen from a dropdown
    }

    public PuzzlePieceType GetPieceType()
    {
        return _pieceType;
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

public enum PuzzlePieceType { right, left, up, down, forLoop, conditional, message};

