using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ReturnToLastScreenButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
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
        ScreenManager.Instance.SwitchToPreviousScreen();
    }
}