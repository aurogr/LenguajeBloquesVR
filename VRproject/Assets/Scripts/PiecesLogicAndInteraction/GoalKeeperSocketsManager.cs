using UnityEngine;
using System.Collections;

public class GoalKeeperSocketsManager : SocketsManager
{
    [SerializeField] DefenderSocketsManager _defenderSocketsManager;

    FeedbackScreenImplementation _feedbackScreen;

    void Start() // waypoints controller has to go before
    {
        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>(true);
    }

    override public void StartMovement()
    {
        Debug.Log("[GKSM StartMovement]");
        StartCoroutine(GoalKeeperMovement());
    }

    private IEnumerator GoalKeeperMovement()
    {
        Debug.Log("[GKSM GoalKeeperMovement]");

        int successCounter = 0; // there are two defenders that must reach a position in the map, so the movement will be succesfull when the defenderSocketsManager returns true two times

        for (int i = 0; i < base._sockets.Count; i++) // go through sockets (ignoring last because it doesn't contain a puzzle piece)
        {
            Debug.Log("[GKSM] sockets count = " + _sockets.Count);

            ConditionSetter _currentPieceConditions = base._sockets.Dequeue().GetComponentInChildren<ConditionSetter>(); // we know that this socket manager only has message pieces, which have a condition setter
            int times = _currentPieceConditions.GetConditionTimes();
            GameConditions direction = _currentPieceConditions.GetConditionSide();
            GameConditions color = _currentPieceConditions.GetConditionColor();

            yield return _defenderSocketsManager.StartMovement(color, direction, times);
            bool positionReached = _defenderSocketsManager.GetResult();

            if (positionReached)
                successCounter++;
        }

        if (successCounter == 2)
        {
            _feedbackScreen.PrintFeedbackMessage("olee", true);
        }
        else
        {
            _feedbackScreen.PrintFeedbackMessage("No has colocado a los jugadores", false);
        }
    }
}
