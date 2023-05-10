using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public event Action OnSceneReset;
    public event Action OnGameStart;

    private bool _isGameSituationTheSame;

    public enum GameLevels { BasicLevel, LoopLevel, ConditionalLevel}

    public GameLevels GameLevel;

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
        if (GameManager.Instance != null)  // because we have some gameManagers on some scenes to try quicker, but they shouldn't be on the build
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // gameManager stays between scenes
        }
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

    //public void GameEnd(string message, bool playerSucceeded)
    //{
    //    Debug.Log("[GameManager] GameEnd");
    //    FindObjectOfType<FeedbackScreenImplementation>(true).PrintFeedbackMessage(message, playerSucceeded); // cannot search for this object just once because it will be different in every screen
    //}

    #region Getters
    public bool GetIsGameSituationTheSame()
    {
        return _isGameSituationTheSame;
    }

    #endregion

}
