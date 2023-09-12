using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ChangeSceneButton : MonoBehaviour
{
    [SerializeField] string _nextSceneName;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ChangeScene);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ChangeScene);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
    }
}

