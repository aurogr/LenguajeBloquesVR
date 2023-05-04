using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class SwitchScreenButton : MonoBehaviour
{
    [SerializeField] private string _nextScreenName;
    private Button _button;
    private ScreenManager _screenManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _screenManager = FindObjectOfType<ScreenManager>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(SwitchScreen);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(SwitchScreen);
    }

    private void SwitchScreen()
    {
        _screenManager.SwitchScreen(_nextScreenName);
    }
}