using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderMovement
{
    Vector3 _directionOfMovement;
    Vector3 _targetPosition;
    Transform _transform;

    float _speed;
    float _rotationAngle;
    float _lengthCellGrid;

    public DefenderMovement(Transform transform, float speed, float lenghtCellGrid)
    {
        _transform = transform;
        _targetPosition = transform.position;
        _speed = speed;
        _lengthCellGrid = lenghtCellGrid;
    }

    #region Movement
    public void FixedUpdate() // because the class is not a monobehaviour, unity won't call the fixed update, we have to call it from the class it was instantiated from
    {
        if (_targetPosition != _transform.position)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }

    public void MoveBall(PuzzlePieceType puzzlePieceType)
    {
        //InstantiateTrailBall();

        switch (puzzlePieceType)
        {
            case PuzzlePieceType.right:
                _directionOfMovement = _transform.right;
                break;
            case PuzzlePieceType.left:
                _directionOfMovement = -_transform.right;
                break;
            case PuzzlePieceType.up:
                _directionOfMovement = _transform.up;
                break;
            case PuzzlePieceType.down:
                _directionOfMovement = -_transform.up;
                break;
        }
        _targetPosition += _directionOfMovement * _lengthCellGrid;
    }

    #endregion
}
