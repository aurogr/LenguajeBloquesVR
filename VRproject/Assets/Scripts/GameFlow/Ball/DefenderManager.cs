using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderManager : MonoBehaviour
{
    #region Variables
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _movementDuration = 0.5f;
    
    [SerializeField] GameObject  _defenderGoal; // object that marks where the defender should go to count the level as finished
    
    Vector3 _startPosition; // start position of the defender, placed on the editor
    CustomSocketInteractor _currentSocket;
    DefenderMovement _defenderMovement;
    Vector3 _currentGoalPosition;
    Vector3 _startGoalPosition;

    bool _defenderOnGoal;

    #endregion

    #region Awake, set variables
    void Start() // waypoints controller has to go before
    {
        _startPosition = transform.position; // get editor position on start
        _startGoalPosition = transform.position;
        SetRandomGoalPosition();
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
        ResetBasicLevel();
    }

    void SetRandomGoalPosition()
    {
        _currentGoalPosition = new Vector3(_startGoalPosition.x, _startGoalPosition.y, _startGoalPosition.z + (_lengthCellGrid * Random.Range(-5, 5)));
        _defenderGoal.transform.position = _currentGoalPosition;
    }

    void ResetBasicLevel()
    {
        transform.position = _startPosition;

        if (GameManager.Instance.GetIsGameSituationTheSame())
        {
            _defenderGoal.transform.position = _currentGoalPosition;
        }
        else
        {
            _currentGoalPosition = new Vector3(_startGoalPosition.x, _startGoalPosition.y, _startGoalPosition.z + (_lengthCellGrid * Random.Range(-5, 5)));
            _defenderGoal.transform.position = _currentGoalPosition;
        }
    }

    #endregion

    #region Simulation flow
    public IEnumerator StartMovement(Queue<CustomSocketInteractor> sockets, GameConditions direction, int messageTimes) // called from the sockets manager
    {
        _defenderMovement = new DefenderMovement(transform, _speed, _lengthCellGrid);
        yield return StartCoroutine(MovePuzzlePieces(sockets, direction, messageTimes));
    }

    private IEnumerator MovePuzzlePieces(Queue<CustomSocketInteractor> sockets, GameConditions direction, int messageTimes) // coroutine to move each piece
    {
        yield return new WaitForSeconds(0.5f); // wait a little to start

        bool exitWhileLoop = false;
        // all of this is possible because the sockets are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search
        while (sockets.Count != 1 && !exitWhileLoop) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            switch (_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType())
            {
                case PuzzlePieceType.conditional:
                    yield return StartCoroutine(MoveConditionalPiece(sockets, direction, messageTimes));
                    exitWhileLoop = true; // porque según está planteado después de una pieza condicional no se pueden poner más por lo que al acabar la condición se acaba la simulación
                    break;
                case PuzzlePieceType.forLoop:
                    yield return StartCoroutine(MoveForLoopPiece(sockets, messageTimes));
                    break;
                default:
                    yield return StartCoroutine(MoveNormalPiece(_currentSocket));
                    break;
            }
        }

        _defenderMovement = null;
    }

    public void FixedUpdate()
    {
        if (_defenderMovement != null)
            _defenderMovement.FixedUpdate(); // have to call it manually since it isn't a monobehaviour
    }

    #endregion

    #region End methods

    public bool GetResult()
    {
        return _defenderOnGoal;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("[OnTriggerEnter]");
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            Debug.Log("Defender on goal");
            _defenderOnGoal = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("[OnTriggerExit]");
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            Debug.Log("Defender out of goal");
            _defenderOnGoal = false;
        }
    }

    #endregion

    #region BallPuzzleBehaviour
    public IEnumerator MoveConditionalPiece(Queue<CustomSocketInteractor> sockets, GameConditions direction, int messageTimes)
    {
        // the puzzle pieces of the first condition always have to be dequeued, either to use them or to be able to reach the second condition sockets
        // but the second condition sockets only need to be dequeued if the first condition doesn't apply because conditionals don't admit other pieces after,
        // so it doesn't matter if we leave sockets left on the queue

        CustomSocketInteractor ifConditionSocket = sockets.Peek(); // get first socket inside if condition

        // get all the sockets inside the if condition block in an array
        CustomSocketInteractor[] ifConditionChildrenSockets = ifConditionSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the block)
       
        for (int i = 0; i < ifConditionChildrenSockets.Length; i++) // dequeu the sockets inside the if condition
        {
            sockets.Dequeue();
        }

        if (_currentSocket.GetPuzzlePiece().GetComponent<ConditionSetter>().GetConditionSide() == direction)
        {
            Queue<CustomSocketInteractor> conditionalSockets = new Queue<CustomSocketInteractor>();

            foreach(CustomSocketInteractor socket in ifConditionChildrenSockets)
            {
                conditionalSockets.Enqueue(socket);
            }

            yield return StartCoroutine(MoveInsideConditional(conditionalSockets, messageTimes)); // to wait till the movement is finished to move again
        }
        else
        {
            yield return StartCoroutine(MoveInsideConditional(sockets, messageTimes)); // the remaining sockets are the else condition sockets
        }

        _defenderMovement = null; // when there is no movement active we delete the defenderMovement so it isn't calling the fixedUpdate constantly
    }

    public IEnumerator MoveForLoopPiece(Queue<CustomSocketInteractor> sockets, int messageTimes)
    {
        int forLoopTimes = _currentSocket.GetTimes();

        _currentSocket = sockets.Peek(); // get next socket (which would be the first socket inside the block)

        // get all the sockets inside the block in an array
        CustomSocketInteractor[] blockChildrenSockets = _currentSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the block)
        int numberOfActiveChildrenSockets = blockChildrenSockets.Length - 1; // because the last socket of a block is always empty in case we want to add new pieces

        for (int i = 0; i < blockChildrenSockets.Length; i++) // dequeu the sockets inside the block (including the empty one)
        {
            sockets.Dequeue();
        }

        // move the pieces inside the block
        for (int j = 0; j < messageTimes; j++)
        {
            for (int i = 0; i < numberOfActiveChildrenSockets; i++)
            {
                CustomSocketInteractor currentSocket = blockChildrenSockets[i];

                yield return StartCoroutine(MoveNormalPiece(currentSocket)); // to wait till the movement is finished to move again
            }
        }
    }

    public IEnumerator MoveNormalPiece(CustomSocketInteractor socket)
    {
        for (int i = 1; i <= socket.GetTimes(); i++) // to move it however many times it has been specified on the puzzle piece
        {
            _defenderMovement.MoveBall(socket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType());

            yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again
        }
    }

    private IEnumerator MoveInsideConditional(Queue<CustomSocketInteractor> sockets, int messageTimes)
    {
        while (sockets.Count != 1) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            switch (_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType())
            {
                case PuzzlePieceType.forLoop:
                    yield return StartCoroutine(MoveForLoopPiece(sockets, messageTimes));
                    break;
                default:
                    yield return StartCoroutine(MoveNormalPiece(_currentSocket));
                    break;
            }
        }
    }
    #endregion
}