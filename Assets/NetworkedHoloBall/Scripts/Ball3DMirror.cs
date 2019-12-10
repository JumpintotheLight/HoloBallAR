using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using Mirror;

public class Ball3DMirror : NetworkBehaviour
{
    public float velocityThreshold = 0.01f; //The minimum magnitude needed to add the paddle's force to the ball.
    private bool hasStarted = false;

    //Fields for collision detection
    public LayerMask layerMask = -1;  
    public float skinWidth = 0.1f; 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = this.transform.position;
        Collider c = this.gameObject.GetComponent<Collider>();
        minimumExtent = Mathf.Min(Mathf.Min(c.bounds.extents.x, c.bounds.extents.y), c.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void FixedUpdate()
    {
        if (hasStarted)
        {
            Vector3 movementThisStep = this.transform.position - previousPosition;
            float movementSqrMagnitude = movementThisStep.sqrMagnitude;

            if (movementSqrMagnitude > sqrMinimumExtent)
            {
                float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
                RaycastHit hitInfo;

                //check for obstructions we might have missed 
                if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
                {
                    if (!hitInfo.collider)
                        return;

                    if (hitInfo.collider.isTrigger)
                    {
                        hitInfo.collider.SendMessage("OnTriggerEnter", this.gameObject.GetComponent<Collider>());
                    }

                    if (!hitInfo.collider.isTrigger)
                    {
                        this.transform.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                    }


                }
            }
        
        }

        previousPosition = this.transform.position;
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
            Debug.Log("Ball Collided with Paddle");
            //Paddle3DMirror paddle = collision.gameObject.GetComponent<Paddle3DMirror>();
            //Vector3 normalizedPoint = collision.GetContact(0).normal;
            //this.gameObject.GetComponent<Rigidbody>().AddForce(normalizedPoint * paddle.PaddleVelocity().magnitude + normalizedPoint * paddle.PaddleAngularVelocity().magnitude, ForceMode.VelocityChange);
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
