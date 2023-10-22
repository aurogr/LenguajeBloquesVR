using System.Collections;
using UnityEngine;

public class DefenderSocketsManager : SocketsManager
{
    [SerializeField] DefenderManager _redDefenderManager;
    [SerializeField] DefenderManager _blueDefenderManager;

    public IEnumerator StartMovement(GameConditions color, GameConditions direction, int messageTimes) // different method than father, overload
    {
        base.EnqueueSockets(); // enqueue sockets to use thems

        if (color == GameConditions.Red)
        {
            yield return StartCoroutine(_redDefenderManager.StartMovement(base._sockets, direction, messageTimes));
        }
        else
        {
            yield return StartCoroutine(_blueDefenderManager.StartMovement(base._sockets, direction, messageTimes));
        }
    }

    public bool GetResult()
    {
        return _redDefenderManager.GetResult() && _blueDefenderManager.GetResult(); ;
    }
}
