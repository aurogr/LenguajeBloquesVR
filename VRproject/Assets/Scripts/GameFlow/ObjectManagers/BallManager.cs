using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour, IObjectManager
{
    #region Variables
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _rotationAngleEachFixedUpdate = 10f;
    [SerializeField] float _movementDuration = 0.5f;
    [SerializeField] GameObject _mesh;
    [SerializeField] GameObject _trailSpherePrefab;
    
    Vector3 _center;

    CustomSocketInteractor _currentSocket;
    BallMovement _ballMovement;
    Vector3 _currentPositionStart;
    FeedbackScreenImplementation _feedbackScreen;

    List<GameObject> _waypoints;
    bool _firstTime = true;

    MeshRenderer _renderer;

    private WaitForSeconds waitForSeconds;

    #endregion

    #region Awake, set variables
    void Start() // waypoints controller has to go before
    {
        _center = transform.position;
        _currentPositionStart = transform.position;
        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>(true);
        _renderer = _mesh.GetComponent<MeshRenderer>();

        waitForSeconds = new WaitForSeconds(_movementDuration);

        OnSceneReset(); // first time is played
    }
    #endregion

    #region Reset level

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
        // reset position (and waypoints on a loop level)
        if (GameManager.Instance.GameLevel == GameLevels.LoopLevel)
        {
            ResetLoopLevel();
        }
        else
        {
            ResetBasicLevel();

            // pick between two colors randomly
            GameManager.Instance.GameCondition = Random.Range(1, 3) switch
            {
                1 => GameConditions.Red,
                _ => GameConditions.Blue,
            };

            if (GameManager.Instance.GameLevel == GameLevels.ConditionalLevel)
            {
                _renderer.material.color = new Color(1, 1, 1);
            }
            else
            {
                if (GameManager.Instance.GameCondition == GameConditions.Red)
                    _renderer.material.color = new Color(1, 0, 0);
                else
                    _renderer.material.color = new Color(0, 0, 1);
            }
        }

        // create movement objects
        _ballMovement = new BallMovement(this.gameObject, _mesh, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
    }

    void SetRandomBallPosition()
    {
        _currentPositionStart = new Vector3(_center.x, _center.y + _lengthCellGrid * Random.Range(-2, 2), _center.z + (_lengthCellGrid * Random.Range(-4, 4)));
        transform.position = _currentPositionStart;
    }

    void ResetBasicLevel()
    {
        if (_firstTime || !GameManager.Instance.GetIsGameSituationTheSame())
        {
            SetRandomBallPosition();
        }
        else
        {
            transform.position = _currentPositionStart;
        }

        _firstTime = false;
    }

    void ResetLoopLevel()
    {
        transform.position = _currentPositionStart;

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

    #region Start program, go through instructions
    public void StartProgram(Queue<CustomSocketInteractor> sockets) // called from the StartLever
    {
        if (GameManager.Instance.GameLevel == GameLevels.ConditionalLevel) // set ball material to match condition
        {
            if (GameManager.Instance.GameCondition == GameConditions.Red)
                _renderer.material.color = new Color(1, 0, 0);
            else
                _renderer.material.color = new Color(0, 0, 1);
        }

        StartCoroutine(IterateOverQueue(sockets));
    }

    public IEnumerator WaitForProgram(Queue<CustomSocketInteractor> sockets) // called from the sockets manager
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator IterateOverQueue(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        yield return waitForSeconds; // wait a little to start

        // all of this is possible because the sockets are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search
        while (sockets.Count != 1) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            switch (_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType())
            {
                case PuzzlePieceType.conditional:
                    yield return StartCoroutine(ReadConditionalPiece(sockets));
                    ReachedSimulationEnd();
                    yield break; // porque según está planteado después de una pieza condicional no se pueden poner más por lo que al acabar la condición se acaba la simulación
                case PuzzlePieceType.forLoop:
                    yield return StartCoroutine(ReadLoopPiece(sockets));
                    break;
                default:
                    yield return StartCoroutine(ReadMovementPiece(_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>()));
                    break;
            }
        }

        ReachedSimulationEnd();
    }

    public void FixedUpdate()
    {
        if(_ballMovement != null)
            _ballMovement.FixedUpdate(); // have to call it manually since it isn't a monobehaviour
    }

    #endregion

    #region Victory and defeat conditions, end level

    private void ReachedSimulationEnd()
    {
        // release ball movement objects
        _ballMovement.DestroyBalls();
        _ballMovement = null;

        //GameManager.Instance.GameEnd("No has llegado a la portería", false);
        _feedbackScreen.PrintFeedbackMessage("No quedan instrucciones y no has conseguido tu objetivo", false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // winning condition for basic and conditional level
        if (GameManager.Instance.GameCondition == GameConditions.Red)
        {
            if (collision.gameObject.CompareTag("RedGoal"))
            {
                StopBehaviour();

                _feedbackScreen.PrintFeedbackMessage("El balón ha entrado en la portería", true); // change screen
                return;
            }
            else if (collision.gameObject.CompareTag("BlueGoal"))
            {
                StopBehaviour();

                _feedbackScreen.PrintFeedbackMessage("El balón ha entrado en la portería equivocada", false); // change screen
                return;
            }
        }
        else if (GameManager.Instance.GameCondition == GameConditions.Blue)
        {
            if (collision.gameObject.CompareTag("BlueGoal"))
            {
                StopBehaviour();

                _feedbackScreen.PrintFeedbackMessage("El balón ha entrado en la portería", true); // change screen
                return;
            }
            else if (collision.gameObject.CompareTag("RedGoal"))
            {
                StopBehaviour();

                _feedbackScreen.PrintFeedbackMessage("El balón ha entrado en la portería equivocada", false); // change screen
                return;
            }
        }

        // losing condition on collision for all levels
        if (collision.gameObject.CompareTag("FieldLimits"))
        {
            StopBehaviour();

            _feedbackScreen.PrintFeedbackMessage("Te has salido del campo.", false); // change screen
        } else if (collision.gameObject.CompareTag("Waypoint")) // winning condition for loop levels
        {
            _waypoints.Remove(collision.gameObject);
            collision.gameObject.SetActive(false);

            if (_waypoints.Count == 0) // if the player has cleared all the waypoints he wins
            {
                StopBehaviour();

                _feedbackScreen.PrintFeedbackMessage("Has pasado por todos los cuadrados", true); // change screen
            }
        }
    }

    void StopBehaviour()
    {
        // stop ball and release ball movement objects
        StopAllCoroutines();
        _ballMovement.DestroyBalls();
        _ballMovement = null;
    }

    #endregion

    #region Read pieces
    public IEnumerator ReadConditionalPiece(Queue<CustomSocketInteractor> sockets)
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

        if (_currentSocket.GetPuzzlePiece().GetComponent<ConditionSetter>().GetConditionColor() == GameManager.Instance.GameCondition)
        {

            yield return StartCoroutine(GoThroughBlock(ifConditionChildrenSockets, ifConditionChildrenSockets.Length-1)); // to wait till the movement is finished to move again
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

            yield return StartCoroutine(GoThroughBlock(elseConditionChildrenSockets, elseConditionChildrenSockets.Length-1)); // to wait till the movement is finished to move again
        }
    }

    public IEnumerator ReadLoopPiece(Queue<CustomSocketInteractor> sockets)
    {
        int forLoopTimes = _currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetTimes();

        _currentSocket = sockets.Peek(); // get next socket (which would be the first socket inside the block)

        // get all the sockets inside the block in an array
        CustomSocketInteractor[] blockChildrenSockets = _currentSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the block)
        int numberOfActiveChildrenSockets = blockChildrenSockets.Length - 1; // because the last socket of a block is always empty in case we want to add new pieces

        for (int i = 0; i < blockChildrenSockets.Length; i++) // dequeu the sockets inside the block (including empty socket)
        {
            sockets.Dequeue();
        }

        // move the pieces inside the block
        for (int i = 0; i < forLoopTimes; i++)
        {
            yield return StartCoroutine(GoThroughBlock(blockChildrenSockets, numberOfActiveChildrenSockets)); // to wait till the movement is finished to move again
        }
    }


    // go through all pieces inside a block (loop and conditional path)
    public IEnumerator GoThroughBlock(CustomSocketInteractor[] sockets, int numberOfBlockPieces)
    {
        for (int i = 0; i < numberOfBlockPieces; i++)
        {
            CustomSocketInteractor currentSocket = sockets[i];

            yield return StartCoroutine(ReadMovementPiece(currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>())); // to wait till the movement is finished to move again
        }
    }

    // movement piece behaviour
    public IEnumerator ReadMovementPiece(PuzzlePieceInteractableObject puzzlePiece)
    {
        for (int i = 1; i <= puzzlePiece.GetTimes(); i++) // to move it however many times it has been specified on the puzzle piece
        {
            _ballMovement.SelectNextCell(puzzlePiece.GetPieceType());

            yield return waitForSeconds; // to wait till the movement is finished to move again
        }
    }
    #endregion
}
