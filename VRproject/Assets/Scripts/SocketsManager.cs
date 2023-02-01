using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketsManager : MonoBehaviour
{
    BallMovement _ball;
    Queue<CustomSocketInteractor> _sockets;

    private void Awake()
    {
        _sockets = new Queue<CustomSocketInteractor>();
        _ball = FindObjectOfType<BallMovement>();
    }

    private void Update() // temporarily for debug purposes
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnqueueSockets();
        }
    }

    private void EnqueueSockets() // add every socket object contained on this parent object to a queue
    {
        _sockets.Clear();

        CustomSocketInteractor[] sockets = gameObject.GetComponentsInChildren<CustomSocketInteractor>();

        for (int i = 0; i < sockets.Length; i++)
        {
            _sockets.Enqueue(sockets[i]);
        }

        _ball.StartMovement(_sockets); // send queue to ball, it will move based on the puzzle pieces
    }

    


}
