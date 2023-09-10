using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StartLever : MonoBehaviour
{
    XRGrabInteractable _interactionScript;
    [SerializeField] SocketsManager _socketsManager; // given in the editor in the message level, but found in real time in the rest of levels since there is only one
    Rigidbody _rb;
    HingeJoint _hingeJoint;
    bool _ballMovementStarted = false;

    private void Start()
    {
        _interactionScript = gameObject.GetComponent<XRGrabInteractable>();
        _rb = gameObject.GetComponent<Rigidbody>();
        _hingeJoint = gameObject.GetComponent<HingeJoint>();

        if (_socketsManager == null) // if it wasn't given in the editor, find it
            _socketsManager = FindObjectOfType<SocketsManager>();

        // subscribe to game manager "start game" event to reset the ballMovementStarted boolean
        GameManager.Instance.OnSceneReset += ResetBallMovementBoolean;
    }

    private void ResetBallMovementBoolean()
    {
        // reset level interaction
        _interactionScript.enabled = true; // enable the xr interaction script, so that it can be grabbed again
        _hingeJoint.useSpring = true;
        _rb.isKinematic = false; // stop the spring movement

        StartCoroutine(ReenableCollider()); // wait for the spring to go back to position to enable movement
    }

    IEnumerator ReenableCollider()
    {
        yield return new WaitForSeconds(1.5f);
        _ballMovementStarted = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!_ballMovementStarted) // we only start this once
        {
            if (collision.gameObject.CompareTag("LeverCollider"))
            {
                // if the movement cannot be activated (the puzzle pieces aren't valid) then,
                // because the hinge is configured as a spring, it will snap back into the initial position
                // else, it will stay at the bottom, showing the player visually that the lever has been triggered

                _ballMovementStarted = _socketsManager.ActivateLever(); // check if the ball movement can be activated (there are sockets)

                if (_ballMovementStarted)
                {
                    _interactionScript.enabled = false; // disable the xr interaction script, so that it can't be grabbed again
                    _hingeJoint.useSpring = false;
                    _rb.velocity = Vector3.zero;
                    _rb.isKinematic = true; // stop the spring movement

                    _socketsManager.StartMovement(); // Start the ball movement from the sockets manager
                }
            }
        }
    }
}
