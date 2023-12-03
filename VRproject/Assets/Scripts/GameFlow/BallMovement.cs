using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement
{
    GameObject _ballSphere;
    GameObject _parent;

    Vector3 _directionOfMovement;
    Vector3 _directionOfRotation;
    Vector3 _targetPosition;
    Transform _transform;
    List<GameObject> _trailSpheres;

    float _speed;
    float _rotationAngle;
    float _lengthCellGrid;

    public BallMovement(GameObject parent, GameObject ballSphere, Transform transform, float speed, float rotationAngle, float lenghtCellGrid)
    {
        _parent = parent;
        _ballSphere = ballSphere;
        _transform = transform;
        _targetPosition = transform.position;
        _speed = speed;
        _rotationAngle = rotationAngle;
        _lengthCellGrid = lenghtCellGrid;

        _trailSpheres = new List<GameObject>();
    }

    #region Movement

    // The fixed update will move the ball in the specified direction until it reaches the position
    public void FixedUpdate() // because the class is not a monobehaviour, unity won't call the fixed update, we have to call it from the class it was instantiated from
    {
        if (_targetPosition != _transform.position)
        {
            _ballSphere.transform.RotateAround(_transform.position, _directionOfRotation, _rotationAngle);
            _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, _speed * Time.deltaTime);
        }
    }

    public void SelectNextCell(PuzzlePieceType puzzlePieceType)
    {
        InstantiateTrailBall();

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
        RecyclableObject ballTrailInstance = _parent.GetComponent<ObjectPoolSpawner>().SpawnObject();

        ballTrailInstance.gameObject.transform.position = _ballSphere.transform.position;
        ballTrailInstance.gameObject.transform.rotation = _ballSphere.transform.rotation;
        ballTrailInstance.GetComponent<Renderer>().material.renderQueue = 3100;

        //GameObject sphere = _mono.Instantiate(_trailSpherePrefab, _ballSphere.transform.position, _ballSphere.transform.rotation, null);
        _trailSpheres.Add(ballTrailInstance.gameObject);

        float numberOfSpheresInTrail = _trailSpheres.Count;
        float counter = 0;

        Color color = _ballSphere.GetComponent<Renderer>().material.color;

        foreach (GameObject trailSphere in _trailSpheres) // gradually change the transparency of the spheres in the trail, old movements should be more transparent
        {
            counter++;
            trailSphere.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, (counter / numberOfSpheresInTrail));
        }
    }

    public void DestroyBalls()
    {
        foreach (GameObject trailSphere in _trailSpheres) // gradually change the transparency of the spheres in the trail, old movements should be more transparent
        {
            trailSphere.GetComponent<BallTrailObject>().Recycle();
        }
    }

    #endregion
}
