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
        StartCoroutine(GoalKeeperMovement());
    }

    private IEnumerator GoalKeeperMovement()
    {
        while(base._sockets.Count != 1) // go through sockets (ignoring last because it doesn't contain a puzzle piece)
        {
            ConditionSetter _currentPieceConditions = base._sockets.Dequeue().GetComponentInChildren<ConditionSetter>(); // we know that this socket manager only has message pieces, which have a condition setter
            int times = _currentPieceConditions.GetConditionTimes();
            GameConditions direction = _currentPieceConditions.GetConditionSide();
            GameConditions color = _currentPieceConditions.GetConditionColor();

            yield return _defenderSocketsManager.StartMovement(color, direction, times);
        }

        if (_defenderSocketsManager.GetResult())
        {
            _feedbackScreen.PrintFeedbackMessage("Los jugadores están donde indica el portero", true);
        }
        else
        {
            _feedbackScreen.PrintFeedbackMessage("No has colocado a los jugadores", false);
        }
    }
}
