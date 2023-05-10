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
    [SerializeField] Vector3 _center;

    CustomSocketInteractor _currentSocket;
    BallMovement _ballMovement;
    BallPuzzleBehaviour _ballPuzzleBehaviour;
    Vector3 _currentBallPositionStart;
    FeedbackScreenImplementation _feedbackScreen;

    List<GameObject> _waypoints;
    private bool _firstTime = true;

    #endregion

    void Awake()
    {
        _currentBallPositionStart = transform.position;
        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>(true);
    }

    #region ResetLevel
    public void OnEnable() // ball is enabled when the scene is reseted (including the first time, just after awake)
    {
        // reset the ball position and the waypoints
        if (GameManager.Instance.GameLevel == GameManager.GameLevels.BasicLevel)
        {
            ResetBasicLevel();
        }
        else
        {
            ResetLoopLevel();
        }

        // create movement objects
        _ballMovement = new BallMovement(_ballSphere, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
        _ballPuzzleBehaviour = new BallPuzzleBehaviour(_ballMovement, _movementDuration);
    }

    void SetRandomBallPosition()
    {
        _currentBallPositionStart = new Vector3(_center.x, _center.y + _lengthCellGrid * Random.Range(-5, 5), _center.z + (_lengthCellGrid * Random.Range(-10, 10)));
        transform.position = _currentBallPositionStart;
    }

    void ResetBasicLevel()
    {
        if (_firstTime || !GameManager.Instance.GetIsGameSituationTheSame())
        {
            SetRandomBallPosition();
        }
        else
        {
            transform.position = _currentBallPositionStart;
        }

        _firstTime = false;
    }

    void ResetLoopLevel()
    {
        transform.position = _currentBallPositionStart;

        if (_firstTime || !GameManager.Instance.GetIsGameSituationTheSame())
        {
            _waypoints = WaypointsController.Instance.GetNewWaypoints();
        }
        else
        {
            _waypoints = WaypointsController.Instance.RepeatWaypoints();
            
        }

        _firstTime = false;
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

    public void FixedUpdate()
    {
        _ballMovement.FixedUpdate(); // have to call it manually since it isn't a monobehaviour
    }

    #endregion

    #region End methods

    private void ReachedSimulationEnd()
    {
        //GameManager.Instance.GameEnd("No has llegado a la portería", false);
        _feedbackScreen.PrintFeedbackMessage("No has llegado a la portería", false);

        // release ball movement objects
        _ballPuzzleBehaviour = null;
        _ballMovement = null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            //GameManager.Instance.GameEnd("Enhorabuena!.", true);
            _feedbackScreen.PrintFeedbackMessage("Enhorabuena!.", true); // change screen

            // stop ball and release ball movement objects
            StopCoroutine("MovePuzzlePieces");
            _ballPuzzleBehaviour = null;
            _ballMovement = null;
        } 
        else if (collision.gameObject.CompareTag("FieldLimits")) {
            //GameManager.Instance.GameEnd("Te has salido del campo. Prueba otra vez.", false);
            _feedbackScreen.PrintFeedbackMessage("Te has salido del campo. Prueba otra vez.", false); // change screen

            // stop ball and release ball movement objects
            StopCoroutine("MovePuzzlePieces");
            _ballPuzzleBehaviour = null;
            _ballMovement = null;
        } else if (collision.gameObject.CompareTag("Waypoint"))
        {
            _waypoints.Remove(collision.gameObject);
            collision.gameObject.SetActive(false);

            if (_waypoints.Count == 0) // if the player has cleared all the waypoints he wins
            {
                //GameManager.Instance.GameEnd("Enhorabuena!.", true);
                _feedbackScreen.PrintFeedbackMessage("Enhorabuena!.", true); // change screen

                // stop ball and release ball movement objects
                StopCoroutine("MovePuzzlePieces");
                _ballPuzzleBehaviour = null;
                _ballMovement = null;
            }
        }
    }

    #endregion
}
