using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using System.Collections.Generic;

public class StartLever : MonoBehaviour
{
    XRGrabInteractable _interactionScript;
    [SerializeField] Button _pauseBtn; // needs to be given in the editor in all scenes, to stop the player from pausing the game while simulation is running (not possible)
    [SerializeField] ProgramPiecesContainer _piecesContainer; // given in the editor in the message level, but found in real time in the rest of levels since there is only one
    [SerializeField] GameObject _programableObject; // needs to be given in the editor in all scenes
    Rigidbody _rb;
    HingeJoint _hingeJoint;
    bool _startedExecution = false;

    private void Start()
    {
        _interactionScript = gameObject.GetComponent<XRGrabInteractable>();
        _rb = gameObject.GetComponent<Rigidbody>();
        _hingeJoint = gameObject.GetComponent<HingeJoint>();

        if (_piecesContainer == null) // if it wasn't given in the editor, find it
            _piecesContainer = FindObjectOfType<ProgramPiecesContainer>();

        // subscribe to game manager "start game" event to reset the ballMovementStarted boolean
        GameManager.Instance.OnSceneReset += ResetAfterExecution;
    }

    private void ResetAfterExecution()
    {
        // reset level interaction
        _pauseBtn.interactable = true;
        _interactionScript.enabled = true; // enable the xr interaction script, so that it can be grabbed again
        _hingeJoint.useSpring = true;
        _rb.isKinematic = false; // stop the spring movement

        // restart socket interaction
        //foreach (var socket in _sockets)
        //{
        //    socket.GetPuzzlePiece().GetComponent<CustomXRGrabInteractable>().enabled = true;
        //}

        StartCoroutine(ReenableCollider()); // wait for the spring to go back to position to enable movement
    }

    IEnumerator ReenableCollider()
    {
        yield return new WaitForSeconds(1.5f);
        _startedExecution = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!_startedExecution && ScreenManager.Instance.GetCurrentScreen() == ScreenName.Game.ToString()) // we only start this once and if the screen is game screen
        {
            if (collision.gameObject.CompareTag("LeverCollider"))
            {
                // if the movement cannot be activated (there are no puzzle pieces in the start sockets) then,
                // because the hinge is configured as a spring, it will snap back into the initial position
                // else, it will stay at the bottom, showing the player visually that the lever has been triggered

                Queue<PuzzlePieceInteractableObject>  puzzlePieces = _piecesContainer.EnqueuePieces();

                if (puzzlePieces.Peek() != null)
                {
                    _startedExecution = true;
                    IObjectManager objectManager = _programableObject.GetComponent(typeof(IObjectManager)) as IObjectManager;
                    objectManager.StartProgram(puzzlePieces); // Start the program

                    // to leave the lever fixed to the bottom
                    _pauseBtn.interactable = false;
                    _interactionScript.enabled = false; // disable the xr interaction script, so that it can't be grabbed again
                    _hingeJoint.useSpring = false;
                    _rb.velocity = Vector3.zero;
                    _rb.isKinematic = true; // stop the spring movement

                    // stop sockets interaction
                    //int counter = 1;
                    //foreach (var socket in _sockets)
                    //{
                    //    if (counter != _sockets.Count)
                    //        socket.GetPuzzlePiece().GetComponent<CustomXRGrabInteractable>().enabled = false;
                    //    counter++;
                    //}
                }
            }
        }
    }
}
