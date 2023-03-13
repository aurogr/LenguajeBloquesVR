using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public event Action OnSceneReset;

    public static GameManager Instance
    {
        get
        {
            // Not handling a null return because the GameManager is the first object created, so this will only happen when the GameManager is destroyed when loading a new screen
            // and other gameobjects being destroyed try to unsubscribe from it's events after (because we can't control the order)
            // This is not a problem, because it is the event owner (gameManager) who is destroyed, so the other gameObjects don't need to unsuscribe anyway
            // (you should unsubscribe if you destroy the subscriber and the event owner lives on, because otherwise the delegates still point to the subscriber)

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void InvokeSceneResetEvent()
    {
        OnSceneReset?.Invoke();
    }

    
}
