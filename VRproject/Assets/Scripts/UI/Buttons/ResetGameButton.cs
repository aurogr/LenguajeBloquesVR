using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ResetGameButton : MonoBehaviour
{
    [SerializeField] bool _sameGameSituation;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ResetGame);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ResetGame);
    }

    private void ResetGame()
    {
        GameManager.Instance.InvokeSceneResetEvent(_sameGameSituation);
        ScreenManager.Instance.SwitchScreen("Game");
    }
}

