using UnityEngine;
using TMPro;

public class FeedbackScreenImplementation : BasicScreenImplementation
{
    TextMeshProUGUI _feedbackText;

    [SerializeField] GameObject _succeedBtns;
    [SerializeField] GameObject _failBtns;

    private void Awake()
    {
        _feedbackText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void PrintFeedbackMessage(string message, bool playerSucceded)
    {
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
