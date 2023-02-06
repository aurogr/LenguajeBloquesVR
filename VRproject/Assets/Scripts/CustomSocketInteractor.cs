using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomSocketInteractor : MonoBehaviour
{
    [SerializeField] GameObject _canvas;
    private XRSocketInteractor _socket;
    private GameObject _puzzlePiece;
    private int _times = 1;

    void Start()
    {
        _socket = GetComponent<XRSocketInteractor>();
        _canvas.SetActive(false);
    }

    #region Add / Remove puzzle pieces to socket

    public void AddPuzzlePiece()
    {
        IXRSelectInteractable obj = _socket.GetOldestInteractableSelected();
        _puzzlePiece = obj.transform.gameObject;
        _puzzlePiece.transform.parent = transform.parent;
        _puzzlePiece.GetComponent<InteractableObject>().ActivateSocket();
        _canvas.SetActive(true);

        //ActivateNextSibling();
    }

    public void RemovePuzzlePiece()
    {
        _puzzlePiece.GetComponent<InteractableObject>().DeactivateSocket();
        _puzzlePiece.transform.parent = null;
        _puzzlePiece = null;
        _canvas.SetActive(false);

        //DeactivateNextSibling();
    }

    private void ActivateNextSibling()
    {
        int index = transform.GetSiblingIndex();
        GameObject nextSibling = transform.parent.GetChild(index + 1).gameObject;
        nextSibling.SetActive(true);
    }

    private void DeactivateNextSibling()
    {
        int index = transform.GetSiblingIndex();
        GameObject nextSibling = transform.parent.GetChild(index + 1).gameObject;
        if (nextSibling.GetComponent<CustomSocketInteractor>().GetPuzzlePiece() == null)
            nextSibling.SetActive(false);
    }

    #endregion

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
