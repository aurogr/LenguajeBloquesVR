using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _rotationAngleEachFixedUpdate = 10f;
    [SerializeField] float _movementDuration = 0.5f;
    [SerializeField] GameObject _ballSphere;
    [SerializeField] GameObject _trailSpherePrefab;

    Vector3 _directionOfMovement;
    Vector3 _directionOfRotation;
    Vector3 _targetPosition;

    bool _isBallInGoal = false;

    List<GameObject> _trailSpheres;

    CustomSocketInteractor _currentSocket;

    FeedbackScreenImplementation _feedbackScreen;

    BallMovement _ballMovement;
    #endregion

    void Start()
    {
        _targetPosition = transform.position;
        _trailSpheres = new List<GameObject>();

        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>();

        _ballMovement = new BallMovement(_ballSphere, transform, _speed, _rotationAngleEachFixedUpdate, _lengthCellGrid);
    }

    #region Ball Movement

    public void FixedUpdate()
    {
        _ballMovement.FixedUpdate();
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

    IEnumerator MovePuzzlePieces(Queue<CustomSocketInteractor> sockets) // coroutine to move each piece
    {
        //all of this is possible because the sockets are stored like a tree inside their parent,
        // and the GetComponentsInChildren is a depth-first tipe of search

        while (sockets.Count != 1 && !_isBallInGoal) // when there's only one socket left, it means that we've reached the end (because the last socket is always empty)
        {
            _currentSocket = sockets.Dequeue();

            // for loop behaviour
            if (_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType() == PuzzlePieceType.forLoop)
            {
                int forLoopTimes = _currentSocket.GetTimes();

                _currentSocket = sockets.Peek(); // get next socket (which would be the first socket inside the for loop)

                // get all the sockets inside the for loop in an array
                CustomSocketInteractor[] forLoopChildrenSockets = _currentSocket.gameObject.GetComponentsInChildren<CustomSocketInteractor>(); // this also gets the parent object (the first socket inside the for loop)
                int numberOfActiveChildrenSockets = forLoopChildrenSockets.Length - 1; // because the last socket of a for loop is always empty in case we want to add new pieces

                for (int i = 0; i < forLoopChildrenSockets.Length; i++) // dequeu the sockets inside the for loop
                {
                    sockets.Dequeue();
                }

                // move the pieces inside the for loop
                for (int i = 0; i < forLoopTimes; i++)
                {
                    for (int j = 0; j < numberOfActiveChildrenSockets; j++)
                    {
                        _currentSocket = forLoopChildrenSockets[j];
                        Debug.Log(_currentSocket);
                            
                        for (int z = 1; z <= _currentSocket.GetTimes(); z++) // to move it however many times it has been specified on the puzzle piece
                        {
                            yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again

                            _ballMovement.MoveBall(_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType());
                            InstantiateTrailBall();
                        }

                    }
                }
            }
            else // normal behaviour
            {
                for (int i = 1; i <= _currentSocket.GetTimes(); i++) // to move it however many times it has been specified on the puzzle piece
                {
                    yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again

                    _ballMovement.MoveBall(_currentSocket.GetPuzzlePiece().GetComponent<PuzzlePieceInteractableObject>().GetPieceType());
                    InstantiateTrailBall();
                }
            }
                
        }

        ReachedSimulationEnd();
        
    }

    #endregion

    #region Balls trail
    private void InstantiateTrailBall()
    {
        GameObject sphere = Instantiate(_trailSpherePrefab, _ballSphere.transform.position, _ballSphere.transform.rotation, null);
        _trailSpheres.Add(sphere);

        float numberOfSpheresInTrail = _trailSpheres.Count;
        float counter = 0;

        foreach(GameObject trailSphere in _trailSpheres) // gradually change the transparency of the spheres in the trail, old movements should be more transparent
        {
            counter++;
            trailSphere.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f * (counter/numberOfSpheresInTrail));
        }
    }

    #endregion

    #region End methods

    private void ReachedSimulationEnd()
    {
        if (!_isBallInGoal)
            SimulationEnd("No has llegado a la portería", false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            _isBallInGoal = true;
            SimulationEnd("Te has salido del campo. Prueba otra vez.", true);
        } else if (collision.gameObject.CompareTag("FieldLimits")) {
            SimulationEnd("Te has salido del campo. Prueba otra vez.", false);
        }
    }

    private void SimulationEnd(string message, bool playerSucceeded)
    {
        _feedbackScreen.PrintFeedbackMessage(message, playerSucceeded);
        _feedbackScreen.Show();
    }

    #endregion
}
