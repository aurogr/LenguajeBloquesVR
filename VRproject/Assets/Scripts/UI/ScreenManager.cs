using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    // this object has the logic of how all the screens (under the same canvas) show and hide
    // it should be on the canvas object, and every screen under that canvas needs to have an script component with the IScreen interface

    [SerializeField] private string _firstShownScreenName; // the screen that will first be visible as we initiliaze the game
    private IDictionary<string, IScreen> _screens;
    private IScreen _currentScreen;
    private IScreen _previousScreen; // if a more complex logic was needed to go through multiple previous screens, we could use a stack

    private void Awake()
    {
        // store all the screens (even the inactive ones) in a dictionary
        _screens = new Dictionary<string, IScreen>();
        IScreen[] screensUnderManager = GetComponentsInChildren<IScreen>(true);

        foreach (IScreen screen in screensUnderManager)
        {
            screen.GetGameObject().SetActive(false);
            _screens.Add(screen.GetName(), screen);
        }

        SwitchScreen(_firstShownScreenName); // show the first screen
    }

    public void SwitchScreen(string screenName)
    {
        // try to find the screen by its name on the dictionary
        IScreen screenToSwitch;
        bool foundScreen = _screens.TryGetValue(screenName, out screenToSwitch);

        if (!foundScreen)
        {
            return; // if the screen wasn't found, return without switching
        }

        SwitchScreen(screenToSwitch); // else, call method that switches the given screen
    }

    private void SwitchScreen(IScreen newScreen)
    {
        if (_currentScreen != null) // if there's a screen showing at the moment of the change hide it and store it as the previous screen
        {
            _currentScreen.Hide();
            _previousScreen = _currentScreen;
        }

        _currentScreen = newScreen;
        _currentScreen.Show();
    }

    public void SwitchToPreviousScreen()
    {
        if (_previousScreen != null)
        {
            SwitchScreen(_previousScreen);
        }
    }
}
