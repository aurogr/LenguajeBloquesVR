using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForLoopPieceBehaviour : MonoBehaviour
{
    [SerializeField] MeshRenderer _puzzlePieceExample;
    [SerializeField] GameObject _endMeshPiece;
    [SerializeField] GameObject _endSocketObject;
    XRSocketInteractor _loopSocket;

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
        _loopSocket = GetComponent<XRSocketInteractor>();
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

        MoveEndPiece();
    }

    private void MoveEndPiece()
    {

        if(_numberOfChildren > 1)
        {
            float offset = _pieceSize * (_numberOfChildren - 1);
            _endMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x + offset, _endMeshPiece.transform.localPosition.y, _endMeshPiece.transform.localPosition.z);
            _endSocketObject.transform.localPosition = new Vector3(_endSocketObjectDefaultLocalPos.x + offset, _endSocketObject.transform.localPosition.y, _endSocketObject.transform.localPosition.z);
        }
        else
        {
            _endMeshPiece.transform.localPosition = new Vector3(_endMeshPieceDefaultLocalPos.x, _endMeshPiece.transform.localPosition.y, _endMeshPiece.transform.localPosition.z);
            _endSocketObject.transform.localPosition = new Vector3(_endSocketObjectDefaultLocalPos.x, _endSocketObject.transform.localPosition.y, _endSocketObject.transform.localPosition.z);
        }
        
    }
}
