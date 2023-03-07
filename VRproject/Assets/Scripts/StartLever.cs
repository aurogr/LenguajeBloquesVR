using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StartLever : MonoBehaviour
{
    XRGrabInteractable _interactionScript;
    SocketsManager _socketsManager; // we can find it in real time instead of assigning it in the editor because there is only one
    Rigidbody _rb;
    HingeJoint _hingeJoint;
    bool _ballMovementStarted = false;


    private void Start()
    {
        _interactionScript = gameObject.GetComponent<XRGrabInteractable>();
        _rb = gameObject.GetComponent<Rigidbody>();
        _hingeJoint = gameObject.GetComponent<HingeJoint>();
        _socketsManager = FindObjectOfType<SocketsManager>();
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

                _ballMovementStarted = _socketsManager.StartBallMovement(); // Start the ball movement from the sockets manager, returns a boolean indicating if it was possible
                
                if (_ballMovementStarted)
                {
                    _interactionScript.enabled = false; // disable the xr interaction script, so that it can't be grabbed again
                    _hingeJoint.useSpring = false;
                    _rb.velocity = Vector3.zero;
                    _rb.isKinematic = true; // stop the spring movement
                }
            }
        }
    }
}
