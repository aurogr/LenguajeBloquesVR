using UnityEngine;
using System;

public class LoopsHelpLevelManager : MonoBehaviour
{
    static LoopsHelpLevelManager _instance;

    public event Action OnNumbersHelpChanged;
    public event Action OnLinesHelpChanged;
    public event Action OnArrowsHelpChanged;

    bool _linesActivated = false;
    bool _arrowsActivated = false;
    bool _specialMaterialActivated = false;

    #region Singleton definition
    public static LoopsHelpLevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    #region Getters/Setters
    public void SetLinesActivated(bool linesActivated)
    {
        _linesActivated = linesActivated;
        OnLinesHelpChanged?.Invoke();
    }

    public void SetArrowsActivated(bool arrowsActivated)
    {
        _arrowsActivated = arrowsActivated;
        OnArrowsHelpChanged?.Invoke();
    }

    public void SetSpecialMaterialActivated(bool specialMaterialActivated)
    {
        _specialMaterialActivated = specialMaterialActivated;
        OnNumbersHelpChanged?.Invoke();
    }

    public bool GetLinesActivated()
    {
        return _linesActivated;
    }

    public bool GetArrowsActivated()
    {
        return _arrowsActivated;
    }

    public bool GetSpecialMaterialActivated()
    {
        return _specialMaterialActivated;
    }
    #endregion
}