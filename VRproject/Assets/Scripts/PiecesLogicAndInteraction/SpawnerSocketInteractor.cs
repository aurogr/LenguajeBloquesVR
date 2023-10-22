using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnerSocketInteractor: MonoBehaviour
{
    ObjectPoolSpawner _spawner;
    XRSocketInteractor _socket;
    IXRSelectInteractable _lastObjectToEnter;

    private void Awake()
    {
        _spawner = gameObject.GetComponentInParent<ObjectPoolSpawner>();
        _socket = gameObject.GetComponent<XRSocketInteractor>();
    }

    public void OnSelectEnter()
    {
        // deactivate socket of the puzzle piece while it is on the spawner
        _lastObjectToEnter = _socket.GetOldestInteractableSelected();

        XRSocketInteractor socket = _lastObjectToEnter.transform.gameObject.GetComponentInChildren<XRSocketInteractor>();
        if (socket != null)
            socket.socketActive = false;
    }

    public void OnSelectExit()
    {
        if (_socket.isActiveAndEnabled) // avoid errors derivated by the socket not being active when exiting the scene because of the order of execution in unity
        {
            StartCoroutine(SpawnObjectWithDelay());

            // activate socket of the puzzle piece when it leaves the spawner
            XRSocketInteractor socket = _lastObjectToEnter.transform.gameObject.GetComponentInChildren<XRSocketInteractor>();
            if (socket != null)
                socket.socketActive = true;
        }
    } 

    IEnumerator SpawnObjectWithDelay()
    {
        _socket.socketActive = false;

        yield return new WaitForSeconds(0.5f);

        _socket.socketActive = true;

        _spawner.SpawnObject();
    }
}
