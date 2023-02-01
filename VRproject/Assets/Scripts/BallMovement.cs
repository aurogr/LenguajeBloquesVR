using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    [SerializeField] float _rotationAngleEachFixedUpdate = 10f;

    Vector3 _directionOfMovement;
    Vector3 _directionOfRotation;
    Vector3 _targetPosition;

    bool _isBallInGoal = false;
    float _movementDuration = 0.5f;

    #region Ball Movement
    void Start()
    {
        _targetPosition = transform.position;
    }

    public void FixedUpdate()
    {
        if (_targetPosition != transform.position)
        {
            transform.RotateAround(transform.position, _directionOfRotation, _rotationAngleEachFixedUpdate);
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

    public void StartMovement(Queue<CustomSocketInteractor> sockets) // called from the sockets manager
    {
        StartCoroutine(MovePuzzlePieces(sockets));
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
                    MoveBall(socket.GetPuzzlePiece().GetComponent<InteractableObject>().GetPieceType());

                    yield return new WaitForSeconds(_movementDuration); // to wait till the movement is finished to move again
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

    #region End methods

    private void OnCollisionEnter(Collision collision)
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
