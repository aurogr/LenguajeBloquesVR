using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class SwitchScreenButton : MonoBehaviour
{
    [SerializeField] string _nextScreenName;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
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
        ScreenManager.Instance.SwitchScreen(_nextScreenName);
    }
}