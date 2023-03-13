using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ReturnToLastScreenButton : MonoBehaviour
{
    private Button _button;
    private ScreenManager _screenManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _screenManager = FindObjectOfType<ScreenManager>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ReturnToScreen);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ReturnToScreen);
    }

    private void ReturnToScreen()
    {
        _screenManager.SwitchToPreviousScreen();
    }
}