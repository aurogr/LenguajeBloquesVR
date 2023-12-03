using System.Collections.Generic;
using UnityEngine;

public class ProgramPiecesContainer : MonoBehaviour
{
    Queue<PuzzlePieceInteractableObject> _puzzlePieces;

    public void Awake()
    {
        _puzzlePieces = new Queue<PuzzlePieceInteractableObject>();
    }

    public Queue<PuzzlePieceInteractableObject> EnqueuePieces() // add every socket object contained on this parent object to a queue
    {
        _puzzlePieces.Clear();

        PuzzlePieceInteractableObject[] puzzlePieces = gameObject.GetComponentsInChildren<PuzzlePieceInteractableObject>();

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            _puzzlePieces.Enqueue(puzzlePieces[i]);
        }

        return _puzzlePieces;
    }
}
