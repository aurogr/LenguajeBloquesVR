using System.Collections.Generic;
using UnityEngine;

public class SocketsManager : MonoBehaviour
{
    // private methods and parameters are not passed to its subclasses, while public ones are.
    // Private methods are exclusive to the normal behaviour of this class (controlling a ball),
    // while public ones will be used by the subclasses of the message scene,
    // virtual methods are overried on the subclass, to still be called with the normal behaviour but change their specifid one

    BallManager _ball;
    [HideInInspector] public Queue<CustomSocketInteractor> _sockets;

    public void Awake()
    {
        _sockets = new Queue<CustomSocketInteractor>();
    }

    private void Start()
    {
        _ball = FindObjectOfType<BallManager>();
    }

    public bool ActivateLever()
    {
        EnqueueSockets();

        if (_sockets.Peek().GetPuzzlePiece() == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    virtual public void StartMovement()
    {
        _ball.StartMovement(_sockets); // send queue to ball, it will move based on the puzzle pieces
    }

    public void EnqueueSockets() // add every socket object contained on this parent object to a queue
    {
        _sockets.Clear();

        CustomSocketInteractor[] sockets = gameObject.GetComponentsInChildren<CustomSocketInteractor>();

        for (int i = 0; i < sockets.Length; i++)
        {
            _sockets.Enqueue(sockets[i]);
        }
    }
}
