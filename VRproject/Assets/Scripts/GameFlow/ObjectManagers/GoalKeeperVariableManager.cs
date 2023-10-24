using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperVariableManager : MonoBehaviour, IObjectManager
{

    #region Variables
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _movementDuration = 0.5f;
    [SerializeField] GameObject _defenderGoal; // object that marks where the defender should go to count the level as finished

    Vector3 _center;
    Vector3 _currentGoalPosition;
    Vector3 _startGoalPosition;

    CustomSocketInteractor _currentSocket;
    Vector3 _targetPosition;
    [SerializeField] Transform _defenderTransform;
    bool _reachedGoal = false;
    bool _defenderOnGoal = false;
    bool _stopIteration = false;
    bool _defenderOnField = true;
    FeedbackScreenImplementation _feedbackScreen;
    private WaitForSeconds waitForSeconds;

    GameConditions? _direction = null;
    int _times = 0;

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
        transform.position = _center;

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
    public void StartProgram(Queue<CustomSocketInteractor> sockets) // called from the StartLever
    {
        StartCoroutine(IterateOverQueue(sockets));
    }

    public IEnumerator WaitForProgram(Queue<CustomSocketInteractor> sockets) // called from the sockets manager
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator IterateOverQueue(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        yield return new WaitForSeconds(0.5f); // wait a little to start

        while (sockets.Count != 1 && !_stopIteration) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();
            PuzzlePieceType puzzlePieceType = _currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType();

            if (puzzlePieceType == PuzzlePieceType.message)
            {
                if (_direction != null && _times != 0) {
                    MoveDefender();
                    for (int i = 0; i <= _times; i++) // wait until movement finishes
                    {
                        yield return waitForSeconds;
                    }
                }
                else
                {
                    _stopIteration = true;
                }
                    
            } else if (puzzlePieceType == PuzzlePieceType.variableTimes)
            {
                _times =  _currentSocket.GetPuzzlePiece().GetComponent<ConditionSetter>().GetConditionTimes();
            } else if (puzzlePieceType == PuzzlePieceType.variableDirection)
            {
                _direction = _currentSocket.GetPuzzlePiece().GetComponent<ConditionSetter>().GetConditionDirection();
            }
        }

        ReachedSimulationEnd();
    }

    public void FixedUpdate() // because the class is not a monobehaviour, unity won't call the fixed update, we have to call it from the class it was instantiated from
    {
        if (_targetPosition != _defenderTransform.position && _defenderOnField)
        {
            _defenderTransform.position = Vector3.MoveTowards(_defenderTransform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }

    private void MoveDefender()
    {
        if (_direction == GameConditions.Left)
        {
            _targetPosition += -_defenderTransform.right * _lengthCellGrid * _times;
        }
        else
        {
            _targetPosition += _defenderTransform.right * _lengthCellGrid * _times;
        }
    }

    #endregion


    #region Victory and defeat conditions, end level

    private void ReachedSimulationEnd()
    {
        if (_defenderOnGoal)
            _feedbackScreen.PrintFeedbackMessage("", true);
        else
        {
            if (_direction == null || _times == 0)
                _feedbackScreen.PrintFeedbackMessage("Asigna un valor a las variables antes de enviar un mensaje al defensa", false);
            else if (!_defenderOnField)
                _feedbackScreen.PrintFeedbackMessage("Te has salido del campo.", false);
            else if (_reachedGoal)
                _feedbackScreen.PrintFeedbackMessage("Te has pasado de casilla", false);
            else if ((_defenderTransform.position.x > _center.x && _currentGoalPosition.x < _center.x) || (_defenderTransform.position.x < _center.x && _currentGoalPosition.x > _center.x))
                _feedbackScreen.PrintFeedbackMessage("Dirección equivocada", false);
            else
                _feedbackScreen.PrintFeedbackMessage("No has llegado a la casilla", false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("[OnTriggerEnter]");
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

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("[OnTriggerExit]");
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            _defenderOnGoal = false;
        }
    }

    #endregion
}
