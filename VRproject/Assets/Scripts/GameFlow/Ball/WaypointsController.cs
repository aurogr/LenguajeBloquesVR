using System.Collections.Generic;
using UnityEngine;

public class WaypointsController : MonoBehaviour
{
    GameObject[] _waypointsOptions;
    Queue<GameObject> _unusedOptions;
    GameObject _currentOption;

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
            // Not handling a null return because the GameManager is the first object created, so this will only happen when the GameManager is destroyed when loading a new screen
            // and other gameobjects being destroyed try to unsubscribe from it's events after (because we can't control the order)
            // This is not a problem, because it is the event owner (gameManager) who is destroyed, so the other gameObjects don't need to unsuscribe anyway
            // (you should unsubscribe if you destroy the subscriber and the event owner lives on, because otherwise the delegates still point to the subscriber)

            return _instance;
        }
    }
    #endregion

    private void Awake() // on awake to initialize it before the ball manager
    {
        _instance = this;

        _waypointsOptions = GameObject.FindGameObjectsWithTag("WaypointsHolder");

        _unusedOptions = new Queue<GameObject>();

        foreach (GameObject option in _waypointsOptions)
        {
            _unusedOptions.Enqueue(option);
            option.SetActive(false);
        }
    }

    public List<GameObject> RepeatWaypoints()
    {
        return GetWaypointsFromCurrentOption();
    }

    public List<GameObject> GetNewWaypoints()
    {
        if (_currentOption != null) // if it isn't the first time
        {
            _currentOption.SetActive(false); // hide the current option
        }

        if (_unusedOptions.Count == 0) // if the queue is empty enqueue all objects again
        {
            foreach (GameObject option in _waypointsOptions)
            {
                _unusedOptions.Enqueue(option);
            }
        }

        // get next option and show it
        _currentOption = _unusedOptions.Dequeue();
        _currentOption.SetActive(true);

        return GetWaypointsFromCurrentOption();
    }

    private List<GameObject> GetWaypointsFromCurrentOption()
    {
        // return waypoints inside current option
        WaypointDummy[] waypoints = _currentOption.GetComponentsInChildren<WaypointDummy>(true);
        List<GameObject> waypointsList = new List<GameObject>();

        foreach (WaypointDummy waypoint in waypoints)
        {
            waypoint.gameObject.SetActive(true); // show waypoints in case they were hidden from previous games
            waypointsList.Add(waypoint.gameObject);
        }

        return waypointsList;
    }


}
