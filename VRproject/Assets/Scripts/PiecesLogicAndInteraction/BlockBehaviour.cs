using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is only given to the socket of a block piece that changes size according to its children so that it can keep count of the puzzle pieces stored inside it
/// </summary>
public class BlockBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject _endMeshPiece;
    [SerializeField] Transform _startTopMeshPiece;
    [SerializeField] Transform _middleTopMeshPiece;
    [SerializeField] Transform _endTopMeshPiece;
    [SerializeField] GameObject _endSocketObject;
    [SerializeField] MeshRenderer _realSizeObject;

    Vector3 _endSocketObjectDefaultLocalPos;
    Vector3 _endMeshPieceDefaultLocalPos;
    float _pieceSize;
    float _numberOfChildren;

    Queue<PuzzlePieceInteractableObject> _lastChildrenPuzzlePieces; // list of the oldest knwon set of pieces inside the block
    #endregion

    #region Start, set variables
    void Start()
    {
        _lastChildrenPuzzlePieces = new Queue<PuzzlePieceInteractableObject>();
        _endMeshPieceDefaultLocalPos = _endMeshPiece.transform.localPosition;
        _endSocketObjectDefaultLocalPos = _endSocketObject.transform.localPosition;
        _pieceSize = _realSizeObject.bounds.size.x; // the length of a puzzle piece, to move the end mesh piece accurately
      
        ChangeScaleTopPiece();
    }
    #endregion

    #region Set block children
    /// <summary>
    /// Get number of times that the piece inside this socket will be executed
    /// </summary>
    public void CheckChildrenPuzzlePieces()
    {
        PuzzlePieceInteractableObject[] childrenPuzzlePieces = GetComponentsInChildren<PuzzlePieceInteractableObject>();

        // removes all children when the inside of the block changes, and checks the inside of the block again to find all remaining/new child pieces and notify them
        for(int i = 0; i < _lastChildrenPuzzlePieces.Count; i++)
        {
            PuzzlePieceInteractableObject puzzlePiece =  _lastChildrenPuzzlePieces.Dequeue();
            puzzlePiece.RemoveFatherBlockPointer(); // tell the pieces inside this block that they are no longer a child of this block
        }
        foreach (PuzzlePieceInteractableObject puzzlePiece in childrenPuzzlePieces)
        {
            puzzlePiece.SetFatherBlockPointer(this); // tell the pieces inside this block that they are inside a block and point them towards the block piece
            _lastChildrenPuzzlePieces.Enqueue(puzzlePiece);
        }

        _numberOfChildren = childrenPuzzlePieces.Length;

        MoveEndAndTopPiece();
    }
    #endregion

    #region Block piece scaling
    private void MoveEndAndTopPiece()
    {
        if(_numberOfChildren > 0)
        {
            float offset = _pieceSize * (_numberOfChildren);
            _endMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x + offset, _endMeshPiece.transform.localPosition.y, _endMeshPiece.transform.localPosition.z);
            _endTopMeshPiece.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x + offset, _endTopMeshPiece.localPosition.y, _endTopMeshPiece.localPosition.z);
            _endSocketObject.transform.localPosition = new Vector3(_endSocketObjectDefaultLocalPos.x + offset, _endSocketObject.transform.localPosition.y, _endSocketObject.transform.localPosition.z);
        }
        else
        {
            _endMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x, _endMeshPiece.transform.localPosition.y, _endMeshPiece.transform.localPosition.z);
            _endTopMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x, _endTopMeshPiece.transform.localPosition.y, _endTopMeshPiece.transform.localPosition.z);
            _endSocketObject.transform.localPosition = new Vector3(_endSocketObjectDefaultLocalPos.x, _endSocketObject.transform.localPosition.y, _endSocketObject.transform.localPosition.z);
        }

        ChangeScaleTopPiece();
    }

    private void ChangeScaleTopPiece()
    {
        // change scale of top piece

        // get the distance between the start and end piece
        float distance = Vector3.Distance(_startTopMeshPiece.localPosition, _endTopMeshPiece.localPosition);

        // the new scale would be the new distance / normal distance (pieceSize) (because the normal distance is with a scale of 1, otherwise we'd have to multiply the new distance by the normal scale too)
        _middleTopMeshPiece.localScale = new Vector3((distance / _pieceSize), _middleTopMeshPiece.transform.localScale.y, _middleTopMeshPiece.transform.localScale.z);

        Vector3 middlePoint = (_startTopMeshPiece.localPosition + _endTopMeshPiece.localPosition) / 2;
        _middleTopMeshPiece.localPosition = middlePoint;
    }

    public void ResetSize()
    {
        _endMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x, _endMeshPiece.transform.localPosition.y, _endMeshPiece.transform.localPosition.z);
        _endTopMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x, _endTopMeshPiece.transform.localPosition.y, _endTopMeshPiece.transform.localPosition.z);
        _endSocketObject.transform.localPosition = new Vector3(_endSocketObjectDefaultLocalPos.x, _endSocketObject.transform.localPosition.y, _endSocketObject.transform.localPosition.z);
 
        ChangeScaleTopPiece();
    }

    #endregion
}
