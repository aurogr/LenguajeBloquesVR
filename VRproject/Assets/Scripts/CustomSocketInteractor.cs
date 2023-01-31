using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomSocketInteractor : MonoBehaviour
{
    private XRSocketInteractor _socket;
    private GameObject _puzzlePiece;
    private int _times = 1;

    void Start()
    {
        _socket = GetComponent<XRSocketInteractor>();
    }

    public void AddPuzzlePiece()
    {
        IXRSelectInteractable obj = _socket.GetOldestInteractableSelected();
        _puzzlePiece = obj.transform.gameObject;

        Debug.Log(obj.transform.name + " in socket of " + transform.name);
    }

    public void RemovePuzzlePiece()
    {
        _puzzlePiece = null;
        Debug.Log("No longer got a puzzle piece in socket of " + transform.name);
    }

    #region Getters/Setters
    public GameObject GetPuzzlePiece()
    {
        return _puzzlePiece;
    }

    public int GetTimes()
    {
        return _times;
    }

    public void SetTimes(int times)
    {
        _times = times + 1; // because the times are chosen from a dropdown
    }

    #endregion
}
