using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForLoopPieceBehaviour : MonoBehaviour
{
    [SerializeField] MeshRenderer _puzzlePieceExample;
    [SerializeField] GameObject _endMeshPiece;
    [SerializeField] Transform _startTopMeshPiece;
    [SerializeField] Transform _middleTopMeshPiece;
    [SerializeField] Transform _endTopMeshPiece;
    [SerializeField] GameObject _endSocketObject;

    Vector3 _endSocketObjectDefaultLocalPos;
    Vector3 _endMeshPieceDefaultLocalPos;
    float _pieceSize;
    float _numberOfChildren;

    Queue<InteractableObject> _lastChildrenPuzzlePieces;

    // Start is called before the first frame update
    void Start()
    {
        _lastChildrenPuzzlePieces = new Queue<InteractableObject>();
        _endMeshPieceDefaultLocalPos = _endMeshPiece.transform.localPosition;
        _endSocketObjectDefaultLocalPos = _endSocketObject.transform.localPosition;
        _pieceSize = _puzzlePieceExample.bounds.size.x; // the length of a puzzle piece, to move the end mesh piece accurately
      
        ChangeScaleTopPiece();
    }

    public void CheckChildrenPuzzlePieces()
    {
        InteractableObject[] childrenPuzzlePieces = GetComponentsInChildren<InteractableObject>();

        // this is probably not optmized, because it removes all children father pointer in every change of the child pieces, and then adds them again if they are still there, should try to find a better solution
        for(int i = 0; i < _lastChildrenPuzzlePieces.Count; i++)
        {
            InteractableObject puzzlePiece =  _lastChildrenPuzzlePieces.Dequeue();
            puzzlePiece.RemoveFatherLoop();
        }
        foreach (InteractableObject puzzlePiece in childrenPuzzlePieces)
        {
            puzzlePiece.SetFatherLoop(this);
            _lastChildrenPuzzlePieces.Enqueue(puzzlePiece);
        }

        _numberOfChildren = childrenPuzzlePieces.Length;

        MoveEndAndTopPiece();
    }

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
        float distance = Vector3.Distance(_startTopMeshPiece.position, _endTopMeshPiece.position);

        _middleTopMeshPiece.localScale = new Vector3(distance, _middleTopMeshPiece.transform.localScale.y, _middleTopMeshPiece.transform.localScale.z);

        Vector3 middlePoint = (_startTopMeshPiece.position + _endTopMeshPiece.position) / 2;
        _middleTopMeshPiece.position = middlePoint;
    }
}
