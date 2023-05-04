using UnityEngine;

public class BasicScreenImplementation : MonoBehaviour, IScreen
{
    [SerializeField] string _name;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetName()
    {
        return _name;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    virtual public void Show()
    {
        gameObject.SetActive(true);
    }
}
