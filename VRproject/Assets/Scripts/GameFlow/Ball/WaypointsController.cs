using System.Collections.Generic;
using UnityEngine;

public class WaypointsController : MonoBehaviour
{
    WaypointsHolder[] _waypointsOptions;
    Queue<WaypointsHolder> _unusedOptions;
    WaypointsHolder _currentOption;

    private static WaypointsController _instance;

    #region Singleton definition
    public static WaypointsController Instance
    {
        get
        {
            if (_instance == null)
            {
                //Debug.Log("There is no instance in the scene");
                return null;
            }

            return _instance;
        }
    }
    #endregion

    private void Awake() // on awake to initialize it before the ball manager
    {
        _instance = this;

        _waypointsOptions = gameObject.GetComponentsInChildren<WaypointsHolder>(true);

        _unusedOptions = new Queue<WaypointsHolder>();

        foreach (WaypointsHolder option in _waypointsOptions)
        {
            _unusedOptions.Enqueue(option);
            option.gameObject.SetActive(false);
        }
    }

    public List<GameObject> RepeatWaypoints()
    {
        return _currentOption.GetWaypointsList();
    }

    public List<GameObject> GetNewWaypoints()
    {
        if (_currentOption != null) // if it isn't the first time
        {
            _currentOption.gameObject.SetActive(false); // hide the current option
        }

        if (_unusedOptions.Count == 0) // if the queue is empty enqueue all objects again
        {
            foreach (WaypointsHolder option in _waypointsOptions)
            {
                _unusedOptions.Enqueue(option);
            }
        }

        // get next option and show it
        _currentOption = _unusedOptions.Dequeue();
        _currentOption.gameObject.SetActive(true);

        return _currentOption.GetWaypointsList();
    }
}
