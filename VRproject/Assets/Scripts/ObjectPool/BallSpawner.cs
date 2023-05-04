using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : ObjectPoolSpawner
{
    private static BallSpawner _instance;

    public static BallSpawner Instance
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
}
