using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderKeeperMessageManager : MonoBehaviour, IObjectManager
{
    #region Variables
    [SerializeField] ProgramPiecesContainer _defenderSocketsManager;

    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _movementDuration = 0.5f;
    [SerializeField] GameObject  _defenderGoal; // object that marks where the defender should go to count the level as finished
    
    Vector3 _center;
    Vector3 _currentGoalPosition;
    Vector3 _startGoalPosition;

    PuzzlePieceInteractableObject _currentPiece;
    Vector3 _directionOfMovement;
    Vector3 _targetPosition;
    bool _reachedGoal = false;
    bool _defenderOnGoal = false;
    bool _defenderOnField = true;
    bool _gameStarted = false;
    FeedbackScreenImplementation _feedbackScreen;
    private WaitForSeconds waitForSeconds;

    #endregion

    #region Awake, set variables
    void Start()
    {
        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>(true);
        _center = transform.position; // get editor position on start
        _startGoalPosition = transform.position;
        _targetPosition = transform.position;
        SetRandomGoalPosition();
        waitForSeconds = new WaitForSeconds(_movementDuration);
    }
    #endregion

    #region ResetLevel

    public void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnSceneReset += OnSceneReset;
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnSceneReset -= OnSceneReset;
    }

    public void OnSceneReset()
    {
        ResetLevel();
    }

    void SetRandomGoalPosition()
    {
        int random = Random.Range(-5, 5);

        while (random == 0) // can't be 0 because then it won't move from start position
            random = Random.Range(-5, 5);

        _currentGoalPosition = new Vector3(_startGoalPosition.x, _startGoalPosition.y, _startGoalPosition.z + (_lengthCellGrid * random));
        _defenderGoal.transform.position = _currentGoalPosition;
    }

    void ResetLevel()
    {
        // reset variables
        transform.position = _center;
        _targetPosition = _center;
        _reachedGoal = false;
        _defenderOnGoal = false;
        _defenderOnField = true;
        _gameStarted = false;

        if (GameManager.Instance.GetIsGameSituationTheSame())
        {
            _defenderGoal.transform.position = _currentGoalPosition;
        }
        else
        {
            SetRandomGoalPosition();
        }
    }

    #endregion

    #region Start program, go through instructions
    public void StartProgram(Queue<PuzzlePieceInteractableObject> goalKeeperSockets)
    {
        Queue<PuzzlePieceInteractableObject> defenderSockets = _defenderSocketsManager.EnqueuePieces();
        if (defenderSockets.Count == 0)
            _feedbackScreen.PrintFeedbackMessage("Programa instrucciones para el defensa", false);
        else
            StartCoroutine(IterateOverQueue(goalKeeperSockets, defenderSockets));
    }

    private IEnumerator IterateOverQueue(Queue<PuzzlePieceInteractableObject> goalKeeperPieces, Queue<PuzzlePieceInteractableObject> defenderPieces) // coroutine to move each piece
    {
        yield return waitForSeconds; // wait a little to start
        _gameStarted = true;

        while (goalKeeperPieces.Count != 0 && _defenderOnField) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            goalKeeperPieces.Dequeue();
            Queue<PuzzlePieceInteractableObject> defenderPiecesCopy = new Queue<PuzzlePieceInteractableObject>(defenderPieces);
            while (defenderPiecesCopy.Count != 0 && _defenderOnField)
            {
                _currentPiece = defenderPiecesCopy.Dequeue();

                for (int i = 0; i < _currentPiece.GetTimes(); i++)
                {
                    SelectNextCell(_currentPiece.GetPieceType());
                    yield return waitForSeconds; // wait a little to start
                }
            }
        }
        ReachedSimulationEnd();
        
    }

    public void FixedUpdate() // because the class is not a monobehaviour, unity won't call the fixed update, we have to call it from the class it was instantiated from
    {
        if (_targetPosition != transform.position && _defenderOnField)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }

    public void SelectNextCell(PuzzlePieceType puzzlePieceType)
    {
        switch (puzzlePieceType)
        {
            case PuzzlePieceType.right:
                _directionOfMovement = transform.right;
                break;
            case PuzzlePieceType.left:
                _directionOfMovement = -transform.right;
                break;
        }
        _targetPosition += _directionOfMovement * _lengthCellGrid;
    }

    #endregion

    #region Victory and defeat conditions, end level

    private void ReachedSimulationEnd()
    {
        if(_defenderOnGoal)
            _feedbackScreen.PrintFeedbackMessage("", true);
        else
        {
            if(!_defenderOnField)
                _feedbackScreen.PrintFeedbackMessage("Te has salido del campo.", false);
            else if (_reachedGoal)
                _feedbackScreen.PrintFeedbackMessage("Te has pasado de casilla", false);
            else if ((transform.position.x > _center.x && _currentGoalPosition.x < _center.x) || (transform.position.x < _center.x && _currentGoalPosition.x > _center.x))
                _feedbackScreen.PrintFeedbackMessage("Dirección equivocada", false);
            else
                _feedbackScreen.PrintFeedbackMessage("No has llegado a la casilla", false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (_gameStarted)
        {
            if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
            {
                _reachedGoal = true;
                _defenderOnGoal = true;
            }
            else if (collision.gameObject.CompareTag("FieldLimits"))
            {
                _defenderOnField = false;
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (_gameStarted)
        {
            if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
            {
                _defenderOnGoal = false;
            }
        }
    }

    #endregion

}