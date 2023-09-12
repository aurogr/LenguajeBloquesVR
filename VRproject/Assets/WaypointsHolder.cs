using System.Collections.Generic;
using UnityEngine;

public class WaypointsHolder : MonoBehaviour
{
    List<GameObject> _waypointsList;
    GameObject _linesHelp;
    GameObject _arrowsHelp;

    #region Awake, enablers
    void Awake()
    {
        SetWaypointsList();
        _linesHelp = GetComponentInChildren<LinesHelpDummy>(true).gameObject;
        _arrowsHelp = GetComponentInChildren<ArrowsHelpDummy>(true).gameObject;
    }

    private void OnEnable()
    {
        LoopsHelpLevelManager.Instance.OnArrowsHelpChanged += AdjustArrowsHelp;
        LoopsHelpLevelManager.Instance.OnLinesHelpChanged += AdjustLinesHelp;
        LoopsHelpLevelManager.Instance.OnNumbersHelpChanged += AdjustNumbersHelp;
        AdjustHelpLevel();
    }

    private void OnDisable()
    {
        if (LoopsHelpLevelManager.Instance != null)
        {
            LoopsHelpLevelManager.Instance.OnArrowsHelpChanged -= AdjustArrowsHelp;
            LoopsHelpLevelManager.Instance.OnLinesHelpChanged -= AdjustLinesHelp;
            LoopsHelpLevelManager.Instance.OnNumbersHelpChanged -= AdjustNumbersHelp;
        }
    }
    #endregion

    #region Help Methods
    void AdjustHelpLevel()
    {
        AdjustNumbersHelp();
        AdjustLinesHelp();
        AdjustArrowsHelp();
    }

    void AdjustNumbersHelp()
    {
        foreach (GameObject waypoint in _waypointsList)
        {
            waypoint.GetComponent<WaypointDummy>().SetMaterial();
        }
    }

    void AdjustArrowsHelp()
    {
        if (LoopsHelpLevelManager.Instance.GetArrowsActivated())
            _arrowsHelp.SetActive(true);
        else
            _arrowsHelp.SetActive(false);
    }

    void AdjustLinesHelp()
    {
        if (LoopsHelpLevelManager.Instance.GetLinesActivated())
            _linesHelp.SetActive(true);
        else
            _linesHelp.SetActive(false);
    }
    #endregion

    #region Getters / Setters
    private void SetWaypointsList()
    {
        _waypointsList = new List<GameObject>();

        // return waypoints inside current option
        WaypointDummy[] waypoints = gameObject.GetComponentsInChildren<WaypointDummy>(true);

        foreach (WaypointDummy waypoint in waypoints)
        {
            waypoint.gameObject.SetActive(true); // show waypoints in case they were hidden from previous games
            _waypointsList.Add(waypoint.gameObject);
        }
    }

    public List<GameObject> GetWaypointsList()
    {
        return _waypointsList;
    }
    #endregion
}
