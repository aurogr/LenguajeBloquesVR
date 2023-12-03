using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ExitGameButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ExitScene);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ExitScene);
    }

    private void ExitScene()
    {
        Application.Quit();
    }
}

