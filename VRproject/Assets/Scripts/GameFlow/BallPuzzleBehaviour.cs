using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPuzzleBehaviour
{
    BallMovement _ballMovement;
    float _movementDuration;

    public BallPuzzleBehaviour(BallMovement ballMovement, float movementDuration)
    {
        _ballMovement = ballMovement;
        _movementDuration = movementDuration;
    }

    public IEnumerator MoveNextPiece(CustomSocketInteractor currentSocket, Queue<CustomSocketInteractor> sockets)
    {
        Debug.Log("[BallPuzzleBehaviour] MoveNextPiece");
        // for loop behaviour
        if (currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType() == PuzzlePieceType.forLoop)
        {
            int forLoopTimes = currentSocket.GetTimes();

            currentSocket = sockets.Peek(); // get next socket (which would be the first socket inside the for loop)

            // get all the sockets inside the for loop in an array
            CustomSocketInteractor[] forLoopChildrenSockets = currentSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the for loop)
            int numberOfActiveChildrenSockets = forLoopChildrenSockets.Length - 1; // because the last socket of a for loop is always empty in case we want to add new pieces

            for (int i = 0; i < forLoopChildrenSockets.Length; i++) // dequeu the sockets inside the for loop
            {
                sockets.Dequeue();
            }

            // move the pieces inside the for loop
            for (int i = 0; i < forLoopTimes; i++)
            {
                for (int j = 0; j < numberOfActiveChildrenSockets; j++)
                {
                    currentSocket = forLoopChildrenSockets[j];
                    Debug.Log(currentSocket);

                    for (int z = 1; z <= currentSocket.GetTimes(); z++) // to move it however many times it has been specified on the puzzle piece
                    {
                        _ballMovement.MoveBall(currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType());

                        yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again
                    }

                }
            }
        }
        else // normal behaviour
        {
            for (int i = 1; i <= currentSocket.GetTimes(); i++) // to move it however many times it has been specified on the puzzle piece
            {
                _ballMovement.MoveBall(currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType());

                yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again
            }
        }
    }
}
