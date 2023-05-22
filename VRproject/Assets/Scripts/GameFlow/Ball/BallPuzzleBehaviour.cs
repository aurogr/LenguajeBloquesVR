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
        PuzzlePieceInteractableObject currentPuzzlePiece = currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>();
        
        // block behaviour
        if ((currentPuzzlePiece.GetPieceType() == PuzzlePieceType.forLoop) || (currentPuzzlePiece.GetPieceType() == PuzzlePieceType.conditional))
        {
            int blockTimes = 1;

            // get block pieces
            if (currentPuzzlePiece.GetPieceType() == PuzzlePieceType.forLoop)
            {
                blockTimes = currentSocket.GetTimes();
            }
            

            currentSocket = sockets.Peek(); // get next socket (which would be the first socket inside the block)

            // get all the sockets inside the block in an array
            CustomSocketInteractor[] blockChildrenSockets = currentSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the block)
            int numberOfActiveChildrenSockets = blockChildrenSockets.Length - 1; // because the last socket of a block is always empty in case we want to add new pieces

            for (int i = 0; i < blockChildrenSockets.Length; i++) // dequeu the sockets inside the block
            {
                sockets.Dequeue();
            }

            if (currentPuzzlePiece.GetPieceType() == PuzzlePieceType.conditional) // if its a conditional see if it can execute, if it can't return
            {
                if (currentPuzzlePiece.GetConditionType() != GameManager.Instance.GameCondition)
                {
                    yield break;
                }
            }                                            
                
            // move the pieces inside the block
            for (int i = 0; i < blockTimes; i++)
            {
                for (int j = 0; j < numberOfActiveChildrenSockets; j++)
                {
                    currentSocket = blockChildrenSockets[j];
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

