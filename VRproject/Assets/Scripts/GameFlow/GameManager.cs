using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public event Action OnSceneReset;
    public event Action OnGameStart;

    private bool _isGameSituationTheSame;
    FeedbackScreenImplementation _feedbackScreen;

    #region Singleton definition
    public static GameManager Instance
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

    private void Awake()
    {
        _instance = this;
        _feedbackScreen = FindObjectOfType<FeedbackScreenImplementation>(true);
    }

    public void InvokeSceneResetEvent(bool isGameSituationTheSame)
    {
        _isGameSituationTheSame = isGameSituationTheSame;
        OnSceneReset?.Invoke();
    }

    public void InvokeGameStartEvent()
    {
        OnGameStart?.Invoke();
    }

    public void GameEnd(string message, bool playerSucceeded)
    {
        Debug.Log("[GameManager] GameEnd");
        _feedbackScreen.PrintFeedbackMessage(message, playerSucceeded);
    }

    #region Getters
    public bool GetIsGameSituationTheSame()
    {
        return _isGameSituationTheSame;
    }

    #endregion

}
