using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is given to all socket objects to define how they interact with the puzzle pieces
/// </summary>
public class ConditionalSizeControl : MonoBehaviour
{
    XRSocketInteractor _socket;
    [SerializeField] GameObject _normalMesh;
    [SerializeField] GameObject _forMesh;
    [SerializeField] Vector3 _normalPosition = Vector3.zero;
    [SerializeField] Vector3 _newPosition = Vector3.zero;

    private void Awake()
    {
        _socket = gameObject.GetComponent<XRSocketInteractor>();
    }

    public void ChangeConditionalMeshBasedOnPuzzlePiece()
    {
        IXRSelectInteractable lastObjectToEnter = _socket.GetOldestInteractableSelected();

        PuzzlePieceInteractableObject ppOnSocket = lastObjectToEnter.transform.gameObject.GetComponent<PuzzlePieceInteractableObject>();

        if (ppOnSocket.GetPieceType() == PuzzlePieceType.forLoop){
            _forMesh.SetActive(true);
            _normalMesh.SetActive(false);

            if(_newPosition != Vector3.zero)
                this.transform.localPosition = _newPosition;
        }
    }

    public void ReturnToNormalMesh()
    {
        _forMesh.SetActive(false);
        _normalMesh.SetActive(true); 
        
        if (_normalPosition != Vector3.zero)
            this.transform.localPosition = _normalPosition;
    }
}