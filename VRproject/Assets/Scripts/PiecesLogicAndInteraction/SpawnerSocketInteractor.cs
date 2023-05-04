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

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += OnGameStartSpawn;
            GameManager.Instance.OnSceneReset += OnResetSceneSpawn;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= OnGameStartSpawn;
            GameManager.Instance.OnSceneReset -= OnResetSceneSpawn;
        }
    }

    private void OnGameStartSpawn()
    {
        Debug.Log("[Spawner socket interactor] On game start spawn");
        _spawner.SpawnObject();
    }

    private void OnResetSceneSpawn()
    {
        _lastObjectToEnter.transform.gameObject.GetComponent<RecyclableObject>().Recycle();
    }

    public void OnSelectEnter()
    {
        // deactivate socket of the puzzle piece while it is on the spawner
        _lastObjectToEnter = _socket.GetOldestInteractableSelected();
        _lastObjectToEnter.transform.gameObject.GetComponentInChildren<XRSocketInteractor>().socketActive = false;
    }

    public void OnSelectExit()
    {
        StartCoroutine(SpawnObjectWithDelay());

        // activate socket of the puzzle piece when it leaves the spawner
        _lastObjectToEnter.transform.gameObject.GetComponentInChildren<XRSocketInteractor>().socketActive = true;
    }

    IEnumerator SpawnObjectWithDelay()
    {
        _socket.socketActive = false;

        yield return new WaitForSeconds(0.5f);

        _socket.socketActive = true;

        _spawner.SpawnObject();
    }
}
