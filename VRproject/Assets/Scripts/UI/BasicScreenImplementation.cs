using UnityEngine;

public class BasicScreenImplementation : MonoBehaviour, IScreen
{
    [SerializeField] string _name;
    GameObject _screenGO;

    private void Awake()
    {
        _screenGO = this.gameObject;
    }

    public GameObject GetGameObject()
    {
        return _screenGO;
    }

    public string GetName()
    {
        return _name;
    }

    public void Hide()
    {
        _screenGO.SetActive(false);
    }

    virtual public void Show()
    {
        _screenGO.SetActive(true);
    }
}
