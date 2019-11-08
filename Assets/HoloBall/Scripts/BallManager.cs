using UnityEngine;
using System.Collections;

public class BallManager : MonoBehaviour
{

    public GameObject BallPrefab;

    //[HideInInspector]
    public GameObject CurrentBall;

    public static BallManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (CurrentBall == null)
            //CurrentBall = Instantiate(BallPrefab, transform.position, transform.rotation) as GameObject;
            CurrentBall = Instantiate(BallPrefab, new Vector3(0, 0.8f, 2.5f), Quaternion.identity) as GameObject;
    }
}