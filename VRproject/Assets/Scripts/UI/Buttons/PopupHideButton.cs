using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class PopupHideButton : MonoBehaviour
{
    [SerializeField] GameObject _popupScreen;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(HidePopup);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(HidePopup);
    }

    private void HidePopup()
    {
        _popupScreen.SetActive(false);
    }
}