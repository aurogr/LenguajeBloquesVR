using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
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
    #endregion

    void Start()
    {
        _targetPosition = transform.position;
        _trailSpheres = new List<GameObject>();
    }

    #region Ball Movement

    public void FixedUpdate()
    {
        if (_targetPosition != transform.position)
        {
            _ballSphere.transform.RotateAround(transform.position, _directionOfRotation, _rotationAngleEachFixedUpdate);
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }

    public void MoveBall(PuzzlePieceType puzzlePieceType)
    {
        switch (puzzlePieceType)
        {
            case PuzzlePieceType.right:
                _directionOfMovement = transform.right;
                _directionOfRotation = -transform.up;
                break;
            case PuzzlePieceType.left:
                _directionOfMovement = -transform.right;
                _directionOfRotation = transform.up;
                break;
            case PuzzlePieceType.up:
                _directionOfMovement = transform.up;
                _directionOfRotation = transform.right;
                break;
            case PuzzlePieceType.down:
                _directionOfMovement = -transform.up;
                _directionOfRotation = -transform.right;
                break;
        }
        _targetPosition += _directionOfMovement * _lengthCellGrid;
    }

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
        bool foundLastOne = false;

        while (!foundLastOne && sockets.Count != 0 && !_isBallInGoal)
        {
            CustomSocketInteractor socket = sockets.Dequeue();

            if (socket.GetPuzzlePiece() != null)
            {
                Debug.Log("Next puzzle piece: " + socket.GetPuzzlePiece().GetComponent<InteractableObject>().GetPieceType() + " times:" + socket.GetTimes());

                for (int i = 1; i <= socket.GetTimes(); i++) // to move it however many times it has been specified on the puzzle piece
                {
                    yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again

                    MoveBall(socket.GetPuzzlePiece().GetComponent<InteractableObject>().GetPieceType());
                    InstantiateTrailBall();
                }
            }
            else
            {
                foundLastOne = true;
            }
        }

        if (!_isBallInGoal)
            EndOutsideGoal();
    }

    #endregion

    #region Trail
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // if the ball collisions with the goal, the player has won
        {
            _isBallInGoal = true;
            ShowEndMenu("GOOL!! :)");
        }
    }

    private void EndOutsideGoal()
    {
        ShowEndMenu("A la siguiente será :(");
        // still to implement: based on the position of the ball, give feedback to player
    }

    private void ShowEndMenu(string message) // we use always the same menu, and change the message given to the player
    {
        Debug.Log(message);
    }

    #endregion
}
