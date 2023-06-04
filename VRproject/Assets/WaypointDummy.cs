using UnityEngine;
using UnityEngine.UI;

public class WaypointDummy : MonoBehaviour
{
    Sprite _helperMat;
    [SerializeField] Sprite _normalMat;

    Image _image;

    public void Awake()
    {
        _image = gameObject.GetComponent<Image>();
        _helperMat = _image.sprite;
    }

    public void SetMaterial()
    {
        bool helpEnabled = LoopsHelpLevelManager.Instance.GetSpecialMaterialActivated();

        if (helpEnabled)
            _image.sprite = _helperMat;
        else
            _image.sprite = _normalMat;
    }
}
