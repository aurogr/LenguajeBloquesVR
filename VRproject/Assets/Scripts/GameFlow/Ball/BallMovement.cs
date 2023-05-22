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
    List<GameObject> _trailSpheres;

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

        _trailSpheres = new List<GameObject>();
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
        //InstantiateTrailBall();

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

    #region Balls trail
    private void InstantiateTrailBall()
    {
        RecyclableObject ballTrailInstance = BallSpawner.Instance.gameObject.GetComponent<ObjectPoolSpawner>().SpawnObject();
        ballTrailInstance.gameObject.GetComponent<BallTrailObject>().SetTrailPosition(_ballSphere.transform);
        //GameObject sphere = _mono.Instantiate(_trailSpherePrefab, _ballSphere.transform.position, _ballSphere.transform.rotation, null);
        _trailSpheres.Add(ballTrailInstance.gameObject);

        float numberOfSpheresInTrail = _trailSpheres.Count;
        float counter = 0;

        foreach (GameObject trailSphere in _trailSpheres) // gradually change the transparency of the spheres in the trail, old movements should be more transparent
        {
            counter++;
            trailSphere.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f * (counter / numberOfSpheresInTrail));
        }
    }

    #endregion
}
