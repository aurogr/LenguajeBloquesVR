using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class StartGameButton : MonoBehaviour
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
        _button.onClick.AddListener(StartGame);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        GameManager.Instance.InvokeGameStartEvent();
        _screenManager.SwitchScreen(_nextScreenName);
    }
}
