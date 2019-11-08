using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using Mirror;

public class Ball3DMirror : NetworkBehaviour
{
    private bool hasStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBall(Vector3 newVelocity)
    {
        if (!hasStarted)
        {
            this.GetComponent<Rigidbody>().velocity = newVelocity;
            hasStarted = true;
        }
    }

    public void OnPlayerDisconnected(NetworkIdentity player)
    {
        GameObject.Destroy(this);
    }
}
