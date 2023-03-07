using UnityEngine;
using TMPro;

public class ShowFeedback : MonoBehaviour
{
    TextMeshProUGUI _feedbackText;

    private void Awake()
    {
        _feedbackText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void PrintMessage(string message)
    {
        _feedbackText.text = message;
    }
}
