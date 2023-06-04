using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class PopupShowButton : MonoBehaviour
{
    [SerializeField] GameObject _popupScreen;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _popupScreen.SetActive(false);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ShowPopup);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ShowPopup);
    }

    private void ShowPopup()
    {
        _popupScreen.SetActive(true);
    }
}