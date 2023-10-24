using UnityEngine;
using TMPro;

public class FeedbackScreenImplementation : BasicScreenImplementation
{
    [SerializeField] GameObject _succeedBtns;
    [SerializeField] GameObject _failBtns;

    TextMeshProUGUI _feedbackText;

    private void Awake()
    {
        _feedbackText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void PrintFeedbackMessage(string message, bool playerSucceded)
    {
        ScreenManager.Instance.SwitchScreen(_name.ToString());

        if (playerSucceded)
        {
            _succeedBtns.SetActive(true);
            _failBtns.SetActive(false);
        }
        else
        {
            _succeedBtns.SetActive(false);
            _failBtns.SetActive(true);
        }

        _feedbackText.text = message;
    }
}
