using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement
{
    GameObject _ballSphere;

    Vector3 _directionOfMovement;
    Vector3 _directionOfRotation;
    Vector3 _targetPosition;
    Transform _transform;

    float _speed;
    float _rotationAngle;
    float _lengthCellGrid;

    public BallMovement(GameObject ballSphere, Transform transform, float speed, float rotationAngle, float lenghtCellGrid)
    {
        _ballSphere = ballSphere;
        _transform = transform;
        _targetPosition = transform.position;
        _speed = speed;
        _rotationAngle = rotationAngle;
        _lengthCellGrid = lenghtCellGrid;
    }

    #region Movement
    public void FixedUpdate() // because the class is not a monobehaviour, unity won't call the fixed update, we have to call it from the class it was instantiated from
    {
        if (_targetPosition != _transform.position)
        {
            _ballSphere.transform.RotateAround(_transform.position, _directionOfRotation, _rotationAngle);
            _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }

    public void MoveBall(PuzzlePieceType puzzlePieceType)
    {
        switch (puzzlePieceType)
        {
            case PuzzlePieceType.right:
                _directionOfMovement = _transform.right;
                _directionOfRotation = -_transform.up;
                break;
            case PuzzlePieceType.left:
                _directionOfMovement = -_transform.right;
                _directionOfRotation = _transform.up;
                break;
            case PuzzlePieceType.up:
                _directionOfMovement = _transform.up;
                _directionOfRotation = _transform.right;
                break;
            case PuzzlePieceType.down:
                _directionOfMovement = -_transform.up;
                _directionOfRotation = -_transform.right;
                break;
        }
        _targetPosition += _directionOfMovement * _lengthCellGrid;
    }

    #endregion
}
