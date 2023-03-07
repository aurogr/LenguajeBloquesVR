using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header ("Menus references")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameOptionsMenu;
    [SerializeField] private GameObject _instructionsMenu;
    [SerializeField] private GameObject _feedbackMenu;

    [Header ("")]
    [SerializeField] public InputActionProperty _menuButton;

    private void Awake()
    {
        _mainMenu.SetActive(false);
    }

    private void Update()
    {
        if (_menuButton.action.WasPressedThisFrame())
        {
            _mainMenu.SetActive(!_mainMenu.activeSelf); // set it active if inactive and viceversa
        }
    }
}
