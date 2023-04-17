using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    #region Variables
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _rotationAngleEachFixedUpdate = 10f;
    [SerializeField] float _movementDuration = 0.5f;
    [SerializeField] GameObject _ballSphere;
    [SerializeField] GameObject _trailSpherePrefab;

    CustomSocketInteractor _currentSocket;
    BallMovement _ballMovement;
    BallPuzzleBehaviour _ballPuzzleBehaviour;

    bool _isBallInGoal = false;
    #endregion

    void Start()
    {
        
        _ballMovement = new BallMovement(_ballSphere, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
        _ballPuzzleBehaviour = new BallPuzzleBehaviour(_ballMovement, _movementDuration, _ballSphere, _trailSpherePrefab);
    }

    public void FixedUpdate()
    {
        _ballMovement.FixedUpdate();
    }

    #region Simulation flow

    public bool StartMovement(Queue<CustomSocketInteractor> sockets) // called from the sockets manager
    {
        if (sockets.Peek().GetPuzzlePiece() == null)
        {
            return false;
        }
        else
        {
            MovePuzzlePieces(sockets);
            return true;
        }
    }

    private void MovePuzzlePieces(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        //all of this is possible because the sockets are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search

        while (sockets.Count != 1 && !_isBallInGoal) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            
            _currentSocket = sockets.Dequeue();
            Debug.Log("i got it here" + _currentSocket.GetPuzzlePiece());
            Debug.Log("i got it" + _currentSocket);

            StartCoroutine(_ballPuzzleBehaviour.MoveNextPiece(_currentSocket, sockets));
        }

        ReachedSimulationEnd();
        
    }

    #endregion

    #region End methods

    private void ReachedSimulationEnd()
    {
        if (!_isBallInGoal)
            GameManager.Instance.GameEnd("No has llegado a la portería", false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            _isBallInGoal = true;
            GameManager.Instance.GameEnd("Te has salido del campo. Prueba otra vez.", true);
        } else if (collision.gameObject.CompareTag("FieldLimits")) {
            GameManager.Instance.GameEnd("Te has salido del campo. Prueba otra vez.", false);
        }
    }

    #endregion
}
