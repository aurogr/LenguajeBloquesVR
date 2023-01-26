using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] float _lengthCellGrid;
    [SerializeField] float _speed;
    Vector3 _directionOfMovement;
    Vector3 _targetPosition;

    void Start()
    {
        _targetPosition = transform.position;
    }

    public void MoveBall(PuzzlePiece.Type puzzlePieceType)
    {
        switch (puzzlePieceType)
        {
            case PuzzlePiece.Type.right:
                _directionOfMovement = transform.right;
                break;
            case PuzzlePiece.Type.left:
                _directionOfMovement = -transform.right;
                break;
            case PuzzlePiece.Type.up:
                _directionOfMovement = transform.up;
                break;
            case PuzzlePiece.Type.down:
                _directionOfMovement = -transform.up;
                break;
        }

        _targetPosition += _directionOfMovement * _lengthCellGrid;

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
    }

    public void Update()
    {
        if (_targetPosition != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }
}
