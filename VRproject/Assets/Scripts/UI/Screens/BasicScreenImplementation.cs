using UnityEngine;

public class BasicScreenImplementation : MonoBehaviour, IScreen
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
        gameObject.SetActive(false);
    }

    virtual public void Show()
    {
        gameObject.SetActive(true);
    }
}
