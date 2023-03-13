using UnityEngine;

public interface IScreen
{
    public GameObject GetGameObject();

    public string GetName();

    public void Show();

    public void Hide();
}
