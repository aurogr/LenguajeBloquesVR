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
    Vector3 _currentBallPositionStart;
    [SerializeField] Vector3 _center;

    public bool IsBallInGoal = false;
    #endregion

    void Start()
    {
        SetRandomBallPosition();
        _ballMovement = new BallMovement(_ballSphere, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
        _ballPuzzleBehaviour = new BallPuzzleBehaviour(_ballMovement, _movementDuration);
    }

    public void FixedUpdate()
    {
        _ballMovement.FixedUpdate();
    }

    #region Ball Position
    public void OnEnable() // ball is enabled when the scene is reseted
    {
        ResetBallPosition();
        _ballMovement = new BallMovement(_ballSphere, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
        _ballPuzzleBehaviour = new BallPuzzleBehaviour(_ballMovement, _movementDuration);
    }

    void SetRandomBallPosition()
    {
        _currentBallPositionStart = new Vector3(_center.x, _center.y + _lengthCellGrid * Random.Range(-5, 5), _center.z + (_lengthCellGrid * Random.Range(-10, 10)));
        transform.position = _currentBallPositionStart;
    }

    void ResetBallPosition()
    {
        if (GameManager.Instance.GetIsGameSituationTheSame())
        {
            transform.position = _currentBallPositionStart;
        }
        else
        {
            SetRandomBallPosition();
        }

    }

    #endregion

    #region Simulation flow

    public bool StartMovement(Queue<CustomSocketInteractor> sockets) // called from the sockets manager
    {
        if (sockets.Peek().GetPuzzlePiece() == null)
        {
            return false;
        }
        else
        {
            StartCoroutine(MovePuzzlePieces(sockets));
            return true;
        }
    }

    private IEnumerator MovePuzzlePieces(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        //all of this is possible because the sockets are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search

        while (sockets.Count != 1) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            yield return StartCoroutine(_ballPuzzleBehaviour.MoveNextPiece(_currentSocket, sockets));
        }
        
        ReachedSimulationEnd();
        
    }

    #endregion

    #region End methods

    private void ReachedSimulationEnd()
    {
        GameManager.Instance.GameEnd("No has llegado a la portería", false);
        _ballPuzzleBehaviour = null;
        _ballMovement = null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            IsBallInGoal = true;
            GameManager.Instance.GameEnd("Olee.", true);
            StopCoroutine("MovePuzzlePieces");
            _ballPuzzleBehaviour = null;
            _ballMovement = null;
        } else if (collision.gameObject.CompareTag("FieldLimits")) {
            GameManager.Instance.GameEnd("Te has salido del campo. Prueba otra vez.", false);

            StopCoroutine("MovePuzzlePieces");
            _ballPuzzleBehaviour = null;
            _ballMovement = null;
        }
    }

    #endregion
}
