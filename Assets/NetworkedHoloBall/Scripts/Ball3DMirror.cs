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


    public void StartBall(Vector3 newVelocity)
    {
        if (!hasStarted)
        {
            this.GetComponent<Rigidbody>().velocity = newVelocity;
            hasStarted = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0.3f), ForceMode.VelocityChange);
    }

    public void OnPlayerDisconnected(NetworkIdentity player)
    {
        NetworkServer.UnSpawn(this.gameObject);
    }
}
