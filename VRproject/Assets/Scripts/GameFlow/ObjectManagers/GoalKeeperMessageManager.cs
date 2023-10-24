using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperMessageManager : MonoBehaviour, IObjectManager
{

    #region Variables
    [SerializeField] SocketsManager _defenderSocketsManager;
    [SerializeField] GameObject _defenderObject;

    Queue<CustomSocketInteractor> _defenderSockets;
    CustomSocketInteractor _currentSocket;
    #endregion


    #region Start program, go through instructions
    public void StartProgram(Queue<CustomSocketInteractor> sockets) // called from the StartLever
    {
        Debug.Log("PROGRAM STARTTED");
        _defenderSockets = _defenderSocketsManager.EnqueueSockets();
        StartCoroutine(IterateOverQueue(sockets));
    }

    public IEnumerator WaitForProgram(Queue<CustomSocketInteractor> sockets) // called from the sockets manager
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator IterateOverQueue(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        yield return new WaitForSeconds(0.5f); // wait a little to start

        while (sockets.Count != 1) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            if (_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType() == PuzzlePieceType.message)
            {
                Debug.Log("HEREEE");
                IObjectManager objectManager = _defenderObject.GetComponent(typeof(IObjectManager)) as IObjectManager;
                yield return (StartCoroutine(objectManager.WaitForProgram(_defenderSockets)));
            }
        }
    }

    #endregion
}
