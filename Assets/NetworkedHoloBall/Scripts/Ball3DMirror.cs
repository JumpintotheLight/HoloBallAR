using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using Mirror;

public class Ball3DMirror : NetworkBehaviour
{
    public float velocityThreshold = 0.01f; //The minimum magnitude needed to add the paddle's force to the ball.
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
        if(collision.gameObject.tag == "Paddle")
        {
            Paddle3DMirror paddle = collision.gameObject.GetComponent<Paddle3DMirror>();
            Vector3 normalizedPoint = collision.GetContact(0).normal;
            this.gameObject.GetComponent<Rigidbody>().AddForce(normalizedPoint * paddle.PaddleVelocity().magnitude + normalizedPoint * paddle.PaddleAngularVelocity().magnitude, ForceMode.VelocityChange);
        }
        

        /*if(collision.gameObject.tag == "PaddleCollider")
        {
            Vector3 pVelocity = collision.gameObject.GetComponent<PaddleCollider>().CurrentVelocity();
            if (Mathf.Sign(pVelocity.z) != Mathf.Sign(this.gameObject.GetComponent<Rigidbody>().velocity.z))
            {
                if (pVelocity.magnitude >= velocityThreshold)
                {
                    this.gameObject.GetComponent<Rigidbody>().AddForce(pVelocity, ForceMode.Impulse);
                }
            }
        }*/

        //this.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0.3f), ForceMode.VelocityChange);
    }

    public void OnPlayerDisconnected(NetworkIdentity player)
    {
        NetworkServer.UnSpawn(this.gameObject);
    }
}
