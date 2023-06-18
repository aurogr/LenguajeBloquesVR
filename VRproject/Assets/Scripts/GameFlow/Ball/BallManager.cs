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
    [SerializeField] Material _redMat;
    [SerializeField] Material _blueMat;
    [SerializeField] Material _normalMat;
    [SerializeField] Vector3 _center;

    CustomSocketInteractor _currentSocket;
    BallMovement _ballMovement;
    Vector3 _currentBallPositionStart;
    FeedbackScreenImplementation _feedbackScreen;

    List<GameObject> _waypoints;
    bool _firstTime = true;

    MeshRenderer _ballRenderer;

    #endregion

    #region Awake, set variables
    void Start() // waypoints controller has to go before
    {
        _currentBallPositionStart = transform.position;
        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>(true);
        _ballRenderer = _ballSphere.GetComponent<MeshRenderer>();

        OnSceneReset(); // first time is played
    }
    #endregion

    #region ResetLevel

    public void OnEnable()
    {
        GameManager.Instance.OnSceneReset += OnSceneReset;
    }

    public void OnDisable()
    {
        GameManager.Instance.OnSceneReset -= OnSceneReset;
    }

    public void OnSceneReset()
    {
        // reset the ball position and the waypoints
        if (GameManager.Instance.GameLevel == GameLevels.LoopLevel)
        {
            ResetLoopLevel();
        }
        else
        {
            ResetBasicLevel();

            if (GameManager.Instance.GameLevel == GameLevels.ConditionalLevel)
            {
                // pick between two colors randomly
                GameManager.Instance.GameCondition = Random.Range(1, 3) switch
                {
                    1 => GameConditions.GoalRed,
                    _ => GameConditions.GoalBlue,
                };

                _ballRenderer.material = _normalMat;
            } else if (GameManager.Instance.GameLevel == GameLevels.MessageLevel)
                {
                    // pick between two colors randomly
                    GameManager.Instance.GameCondition = Random.Range(1, 3) switch
                    {
                        1 => GameConditions.GoalRed,
                        _ => GameConditions.GoalBlue,
                    };

                    _ballRenderer.material = _normalMat;
                }
        }

        // create movement objects
        _ballMovement = new BallMovement(_ballSphere, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
    }

    void SetRandomBallPosition()
    {
        _currentBallPositionStart = new Vector3(_center.x, _center.y + _lengthCellGrid * Random.Range(-4, 4), _center.z + (_lengthCellGrid * Random.Range(-5, 5)));
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
            if (GameManager.Instance.GameLevel == GameLevels.ConditionalLevel) // set ball material to match condition
            {
                if (GameManager.Instance.GameCondition == GameConditions.GoalRed)
                    _ballRenderer.material = _redMat;
                else 
                    _ballRenderer.material = _blueMat;
            }

            StartCoroutine(MovePuzzlePieces(sockets));

            return true;
        }
    }

    private IEnumerator MovePuzzlePieces(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        yield return new WaitForSeconds(0.5f); // wait a little to start

        // all of this is possible because the sockets are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search
        while (sockets.Count != 1) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            switch (_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType())
            {
                case PuzzlePieceType.conditional:
                    yield return StartCoroutine(MoveConditionalPiece(sockets));
                    ReachedSimulationEnd();
                    yield break; // porque según está planteado después de una pieza condicional no se pueden poner más por lo que al acabar la condición se acaba la simulación
                case PuzzlePieceType.forLoop:
                    yield return StartCoroutine(MoveForLoopPiece(sockets));
                    break;
                default:
                    yield return StartCoroutine(MoveNormalPiece(_currentSocket));
                    break;
            }
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
        _ballMovement.DestroyBalls();
        _ballMovement = null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            //GameManager.Instance.GameEnd("Enhorabuena!.", true);
            _feedbackScreen.PrintFeedbackMessage("Enhorabuena!.", true); // change screen

            StopBehaviour();
        } 
        else if (collision.gameObject.CompareTag("FieldLimits")) {
            //GameManager.Instance.GameEnd("Te has salido del campo. Prueba otra vez.", false);
            _feedbackScreen.PrintFeedbackMessage("Te has salido del campo. Prueba otra vez.", false); // change screen

            StopBehaviour();

        } 
        else if (collision.gameObject.CompareTag("Waypoint"))
        {
            _waypoints.Remove(collision.gameObject);
            collision.gameObject.SetActive(false);

            if (_waypoints.Count == 0) // if the player has cleared all the waypoints he wins
            {
                //GameManager.Instance.GameEnd("Enhorabuena!.", true);
                _feedbackScreen.PrintFeedbackMessage("Enhorabuena!.", true); // change screen

                StopBehaviour();
            }
        }
    }

    void StopBehaviour()
    {
        // stop ball and release ball movement objects
        StopCoroutine(nameof(MovePuzzlePieces));
        _ballMovement.DestroyBalls();
        _ballMovement = null;
    }

    #endregion

    #region BallPuzzleBehaviour
    public IEnumerator MoveConditionalPiece(Queue<CustomSocketInteractor> sockets)
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

        if (_currentSocket.GetPuzzlePiece().GetComponent<ConditionSetter>().GetCondition() == GameManager.Instance.GameCondition)
        {

            yield return StartCoroutine(MoveBlock(ifConditionChildrenSockets, ifConditionChildrenSockets.Length)); // to wait till the movement is finished to move again
        }
        else
        {
            CustomSocketInteractor elseConditionSocket = sockets.Peek(); // get first socket inside the else condition

            // get all the sockets inside the else condition block in an array
            CustomSocketInteractor[] elseConditionChildrenSockets = elseConditionSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the block)

            for (int i = 0; i < elseConditionChildrenSockets.Length; i++) // dequeu the sockets inside else condition
            {
                sockets.Dequeue();
            }

            yield return StartCoroutine(MoveBlock(elseConditionChildrenSockets, elseConditionChildrenSockets.Length)); // to wait till the movement is finished to move again
        }
    }

    public IEnumerator MoveForLoopPiece(Queue<CustomSocketInteractor> sockets)
    {
        int forLoopTimes = _currentSocket.GetTimes();

        _currentSocket = sockets.Peek(); // get next socket (which would be the first socket inside the block)

        // get all the sockets inside the block in an array
        CustomSocketInteractor[] blockChildrenSockets = _currentSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the block)
        int numberOfActiveChildrenSockets = blockChildrenSockets.Length - 1; // because the last socket of a block is always empty in case we want to add new pieces

        for (int i = 0; i < numberOfActiveChildrenSockets; i++) // dequeu the sockets inside the block
        {
            sockets.Dequeue();
        }

        // move the pieces inside the block
        for (int i = 0; i < forLoopTimes; i++)
        {
            yield return StartCoroutine(MoveBlock(blockChildrenSockets, numberOfActiveChildrenSockets)); // to wait till the movement is finished to move again
        }
    }

    public IEnumerator MoveNormalPiece(CustomSocketInteractor socket)
    {
        for (int i = 1; i <= socket.GetTimes(); i++) // to move it however many times it has been specified on the puzzle piece
        {
            _ballMovement.MoveBall(socket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType());

            yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again
        }
    }

    public IEnumerator MoveBlock(CustomSocketInteractor[] sockets, int times)
    {
        for (int i = 0; i < times; i++)
        {
            CustomSocketInteractor currentSocket = sockets[i];

            yield return StartCoroutine(MoveNormalPiece(currentSocket)); // to wait till the movement is finished to move again
        }
    }
    #endregion
}
