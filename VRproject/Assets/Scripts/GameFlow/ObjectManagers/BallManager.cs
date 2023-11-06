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

    PuzzlePieceInteractableObject _currentPiece;
    BallMovement _ballMovement;
    Vector3 _currentPositionStart;
    FeedbackScreenImplementation _feedbackScreen;

    List<GameObject> _waypoints;
    bool _firstTime = true;
    bool _specialPieceUsed = false;

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
        _specialPieceUsed = false;

        // reset position (and waypoints on a loop level)
        switch (GameManager.Instance.GameLevel)
        {
            case GameLevels.LoopLevel:
                ResetLoopLevel();
                break;
            case GameLevels.ConditionalLevel:
                ResetConditionalLevel();
                break;
            case GameLevels.BasicLevel:
                ResetBasicLevel();
                break;
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
            GameManager.Instance.GameCondition = Random.Range(1, 3) switch
            {
                1 => GameConditions.Red,
                _ => GameConditions.Blue,
            };

            if (GameManager.Instance.GameCondition == GameConditions.Red)
                _renderer.material.color = new Color(1, 0, 0);
            else
                _renderer.material.color = new Color(0, 0, 1);

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

    void ResetConditionalLevel()
    {
        GameManager.Instance.GameCondition = Random.Range(1, 3) switch
        {
            1 => GameConditions.Red,
            _ => GameConditions.Blue,
        };

        if (_firstTime || !GameManager.Instance.GetIsGameSituationTheSame())
            SetRandomBallPosition();
        else
            transform.position = _currentPositionStart;

        _renderer.material.color = new Color(1, 1, 1);
        _firstTime = false;
    }

    #endregion

    #region Start program, go through instructions
    public void StartProgram(Queue<PuzzlePieceInteractableObject> puzzlePieces) // called from the StartLever
    {
        if (GameManager.Instance.GameLevel == GameLevels.ConditionalLevel) // set ball material to match condition
        {
            if (GameManager.Instance.GameCondition == GameConditions.Red)
                _renderer.material.color = new Color(1, 0, 0);
            else
                _renderer.material.color = new Color(0, 0, 1);
        }

        StartCoroutine(IterateOverQueue(puzzlePieces));
    }

    private IEnumerator IterateOverQueue(Queue<PuzzlePieceInteractableObject> puzzlePieces) // coroutine to move each piece
    {
        yield return waitForSeconds; // wait a little to start

        // all of this is possible because the pieces are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search
        while (puzzlePieces.Count != 0)
        {
            _currentPiece = puzzlePieces.Dequeue();

            switch (_currentPiece.GetPieceType())
            {
                case PuzzlePieceType.conditional:
                    _specialPieceUsed = true;
                    yield return StartCoroutine(ReadConditionalPiece(puzzlePieces));
                    ReachedSimulationEnd();
                    yield break; // porque según está planteado después de una pieza condicional no se pueden poner más por lo que al acabar la condición se acaba la simulación
                case PuzzlePieceType.forLoop:
                    _specialPieceUsed = true;
                    yield return StartCoroutine(ReadLoopPiece(puzzlePieces));
                    break;
                default:
                    yield return StartCoroutine(ReadMovementPiece(_currentPiece));
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

        switch (GameManager.Instance.GameLevel)
        {
            case GameLevels.BasicLevel:
                EndFeedbackBasic();
                break;
            case GameLevels.ConditionalLevel:
                EndFeedbackConditional();
                break;
            case GameLevels.LoopLevel:
                EndFeedbackLoop();
                break;
        }
    }

    private void EndFeedbackBasic()
    {
        string message = "";
        if ((transform.position.z < _currentPositionStart.z && GameManager.Instance.GameCondition == GameConditions.Red) || (transform.position.z > _currentPositionStart.z && GameManager.Instance.GameCondition == GameConditions.Blue))
            message = "Dirección equivocada, fíjate bien en el color de la pelota";
        else if (transform.position.y > (_center.y + _lengthCellGrid*0.5f))
            message = "Baja un poco la pelota";
        else if (transform.position.y < (_center.y - _lengthCellGrid*0.5f))
            message = "Sube un poco la pelota";
        else
            message = "Continúa en esa dirección, ¡casi lo tienes!";

        _feedbackScreen.PrintFeedbackMessage(message, false);
    }

    private void EndFeedbackConditional()
    {
        string message = "";
        if (!_specialPieceUsed)
            message = "Prueba a utilizar la pieza condicional";
        else
            message = "Revisa las piezas utilizadas";

        _feedbackScreen.PrintFeedbackMessage(message, false);
    }

    private void EndFeedbackLoop()
    {
        string message = "";
        if (!_specialPieceUsed)
            message = "Prueba a utilizar la pieza de bucle";
        else
            message = "Revisa las piezas utilizadas";

        _feedbackScreen.PrintFeedbackMessage(message, false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // winning condition for basic and conditional level
        if (GameManager.Instance.GameCondition == GameConditions.Red)
        {
            if (collision.gameObject.CompareTag("RedGoal"))
            {
                StopBehaviour();

                if (GameManager.Instance.GameLevel == GameLevels.ConditionalLevel && !_specialPieceUsed)
                {
                    _feedbackScreen.PrintFeedbackMessage("Has tenido suerte, pero intenta utilizar la pieza condicional la próxima vez", true); // change screen
                }
                else
                {
                    _feedbackScreen.PrintFeedbackMessage("", true); // change screen
                }
                
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

                _feedbackScreen.PrintFeedbackMessage("", true); // change screen
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

                if (!_specialPieceUsed)
                    _feedbackScreen.PrintFeedbackMessage("Lograste tu objetivo, pero intenta utilizar la pieza de bucle la próxima vez", true); // change screen
                else
                    _feedbackScreen.PrintFeedbackMessage("", true); // change screen
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
    public IEnumerator ReadConditionalPiece(Queue<PuzzlePieceInteractableObject> puzzlePieces)
    {
        // the puzzle pieces of the first condition always have to be dequeued, either to use them or to be able to reach the second condition sockets
        // but the second condition sockets only need to be dequeued if the first condition doesn't apply because conditionals don't admit other pieces after,
        // so it doesn't matter if we leave sockets left on the queue

        PuzzlePieceInteractableObject ifFirstPiece = puzzlePieces.Peek(); // get first piece inside if condition

        // get all the pieces inside the if condition block in an array
        PuzzlePieceInteractableObject[] ifPieces = ifFirstPiece.gameObject.GetComponentsInChildren<PuzzlePieceInteractableObject>(); // this also gets the parent object (the first piece inside the block)
       
        for (int i = 0; i < ifPieces.Length; i++) // dequeu the pieces inside the if condition
        {
            puzzlePieces.Dequeue();
        }

        if (_currentPiece.GetComponent<ConditionSetter>().GetConditionColor() == GameManager.Instance.GameCondition)
        {

            yield return StartCoroutine(GoThroughBlock(ifPieces, ifPieces.Length)); // to wait till the movement is finished to move again
        }
        else
        {
            PuzzlePieceInteractableObject elseFirstPiece = puzzlePieces.Peek(); // get first piece inside the else condition

            // get all the pieces inside the else condition block in an array
            PuzzlePieceInteractableObject[] elsePieces = elseFirstPiece.gameObject.GetComponentsInChildren<PuzzlePieceInteractableObject>(); // this also gets the parent object (the first piece inside the block)

            for (int i = 0; i < elsePieces.Length; i++) // dequeu the pieces inside else condition
            {
                puzzlePieces.Dequeue();
            }

            yield return StartCoroutine(GoThroughBlock(elsePieces, elsePieces.Length)); // to wait till the movement is finished to move again
        }
    }

    public IEnumerator ReadLoopPiece(Queue<PuzzlePieceInteractableObject> pieces)
    {
        int forLoopTimes = _currentPiece.GetComponent<PuzzlePieceInteractableObject>().GetTimes();

        _currentPiece = pieces.Peek(); // get next piece (which would be the first socket inside the block)

        // get all the pieces inside the block in an array
        PuzzlePieceInteractableObject[] blockPieces = _currentPiece.gameObject.GetComponentsInChildren<PuzzlePieceInteractableObject>(); // this also gets the parent object (the first socket inside the block)

        for (int i = 0; i < blockPieces.Length; i++) // dequeu the sockets inside the block
        {
            pieces.Dequeue();
        }

        // move the pieces inside the block
        for (int i = 0; i < forLoopTimes; i++)
        {
            yield return StartCoroutine(GoThroughBlock(blockPieces, blockPieces.Length)); // to wait till the movement is finished to move again
        }
    }


    // go through all pieces inside a block (loop and conditional path)
    public IEnumerator GoThroughBlock(PuzzlePieceInteractableObject[] pieces, int numberOfBlockPieces)
    {
        for (int i = 0; i < numberOfBlockPieces; i++)
        {
            PuzzlePieceInteractableObject currentPiece = pieces[i];

            yield return StartCoroutine(ReadMovementPiece(currentPiece)); // to wait till the movement is finished to move again
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
