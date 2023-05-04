using System.Collections.Generic;
using UnityEngine;

public class SocketsManager : MonoBehaviour
{
    BallManager _ball;
    Queue<CustomSocketInteractor> _sockets;

    private void Awake()
    {
        _sockets = new Queue<CustomSocketInteractor>();
        _ball = FindObjectOfType<BallManager>();
    }

    public bool StartBallMovement()
    {
        Debug.Log("[SocketsManager] StartBallMovement");
        EnqueueSockets();

        return _ball.StartMovement(_sockets); // send queue to ball, it will move based on the puzzle pieces
    }

    private void EnqueueSockets() // add every socket object contained on this parent object to a queue
    {
        _sockets.Clear();

        CustomSocketInteractor[] sockets = gameObject.GetComponentsInChildren<CustomSocketInteractor>();

        for (int i = 0; i < sockets.Length; i++)
        {
            _sockets.Enqueue(sockets[i]);
        }
    }
}
