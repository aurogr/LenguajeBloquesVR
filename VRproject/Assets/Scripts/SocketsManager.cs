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

    //private void Start()
    //{
    //    EnqueueSockets();
    //}

    private void Update() // temporarily for debug purposes
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MovePuzzlePieces());
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
    }

    IEnumerator MovePuzzlePieces() // temporarily for debug purposes, but something similar should be used later on to get the pieces
    {
        EnqueueSockets();

        bool foundLastOne = false;

        while (!foundLastOne && _sockets.Count != 0)
        {
            CustomSocketInteractor socket = _sockets.Dequeue();

            if (socket.GetPuzzlePiece() != null)
            {
                Debug.Log("Next puzzle piece: " + socket.GetPuzzlePiece().GetComponent<InteractableObject>().GetPieceType());
                _ball.MoveBall(socket.GetPuzzlePiece().GetComponent<InteractableObject>().GetPieceType());
            }
            else 
            {
                foundLastOne = true;
            }

            yield return new WaitForSeconds(0.5f); // to wait till the movement is finished to move again
        }
    }


}
