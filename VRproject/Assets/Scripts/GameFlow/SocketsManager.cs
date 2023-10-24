using System.Collections.Generic;
using UnityEngine;

public class SocketsManager : MonoBehaviour
{
    Queue<CustomSocketInteractor> _sockets;

    public void Awake()
    {
        _sockets = new Queue<CustomSocketInteractor>();
    }

    public Queue<CustomSocketInteractor> EnqueueSockets() // add every socket object contained on this parent object to a queue
    {
        _sockets.Clear();

        CustomSocketInteractor[] sockets = gameObject.GetComponentsInChildren<CustomSocketInteractor>();

        for (int i = 0; i < sockets.Length; i++)
        {
            _sockets.Enqueue(sockets[i]);
        }

        return _sockets;
    }
}
