using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is given to all socket objects to define how they interact with the puzzle pieces
/// </summary>
public class CustomSocketInteractor : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject _canvas;
    private XRSocketInteractor _socket;
    private GameObject _puzzlePiece;
    private int _times = 1;

    BlockBehaviour _fatherLoop = null;
    #endregion

    #region Start, set variables
    void Start()
    {
        _socket = GetComponent<XRSocketInteractor>();
        _canvas.SetActive(false);
    }
    #endregion

    #region Add / Remove puzzle pieces to socket

    /// <summary>
    /// Executed when a puzzle piece connects to a socket. This socket now has a reference to the puzzle piece inside it
    /// </summary>
    public void AddPuzzlePiece() 
    {
        IXRSelectInteractable obj = _socket.GetOldestInteractableSelected();
        _puzzlePiece = obj.transform.gameObject;
        _puzzlePiece.transform.parent = transform; // set the puzzle piece inside this socket object, so that they can move together and interact based on their heritage

        if (_puzzlePiece.GetComponent<PuzzlePieceInteractableObject>().GetPieceType() != PuzzlePieceType.conditional)
            _canvas.SetActive(true);

        if(_fatherLoop != null) // warn the father loop that a change has been made and it needs to check its children again
        {
            _fatherLoop.CheckChildrenPuzzlePieces();
        }
    }

    /// <summary>
    /// Executed when a puzzle piece disconnects from a socket
    /// </summary>
    public void RemovePuzzlePiece()
    {
        if (_puzzlePiece.GetComponent<CustomXRGrabInteractable>().IsBeingHeld) // the player has removed the puzzle piece
        {
            if (_puzzlePiece.activeSelf)
            {
                _puzzlePiece.transform.parent = null;
            }
        }

        _puzzlePiece = null;
        _canvas.SetActive(false);

        if (_fatherLoop != null)
        {
            _fatherLoop.CheckChildrenPuzzlePieces();
        }
    }

    #endregion

    #region Getters/Setters

    /// <summary>
    /// Get puzzle piece inside this socket (not the puzzle piece that the socket belongs to
    /// </summary>
    public GameObject GetPuzzlePiece()
    {
        return _puzzlePiece;
    }

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

    /// <summary>
    /// This socket now belongs to a piece that is inside a block
    /// </summary>
    /// <param name="fatherLoop" >Give the father block so that it stores a reference to it</param>
    public void SetFatherBlockPointer(BlockBehaviour fatherLoop)
    {
        _fatherLoop = fatherLoop;
    }

    /// <summary>
    /// The piece this socket belongs to is no longer inside a block
    /// </summary>
    public void RemoveFatherBlockPointer()
    {
        _fatherLoop = null;
    }

    public void ActivateSocket()
    {
        _socket.socketActive = true;
    }

    public void DeactivateSocket()
    {
        _socket.socketActive = false;
    }

    #endregion
}
