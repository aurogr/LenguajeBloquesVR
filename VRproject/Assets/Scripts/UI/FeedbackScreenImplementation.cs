using UnityEngine;
using TMPro;

public class FeedbackScreenImplementation : BasicScreenImplementation
{
    [SerializeField] GameObject _succeedBtns;
    [SerializeField] GameObject _failBtns;
    [SerializeField] string _screenName;

    TextMeshProUGUI _feedbackText;
    ScreenManager _screenManager;

    private void Awake()
    {
        _feedbackText = GetComponentInChildren<TextMeshProUGUI>();
        _screenManager = FindObjectOfType<ScreenManager>();
    }

    public void PrintFeedbackMessage(string message, bool playerSucceded)
    {
        //_screenManager.SwitchScreen(_screenName);

        //if (playerSucceded)
        //{
        //    _succeedBtns.SetActive(true);
        //    _failBtns.SetActive(false);
        //}
        //else
        //{
        //    _succeedBtns.SetActive(false);
        //    _failBtns.SetActive(true);
        //}

        //_feedbackText.text = message;
    }
}
