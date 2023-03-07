using UnityEngine;

public interface IScreen
{
    // we'll define a screen as an object on a canvas that has all the information that needs to be displayed at a certain moment
    // (for example, the game menu and options menu are screens)
    public GameObject GetGameObject();

    public string GetName();

    public void Show();

    public void Hide();
}
