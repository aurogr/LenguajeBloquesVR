using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenImplementation : MonoBehaviour, IScreen
{
    [SerializeField] protected ScreenName _name;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetName()
    {
        return _name.ToString();
    }

    public void Hide()
    {
        Renderer[] renderers =  gameObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

    }

    public void Show()
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }
}
