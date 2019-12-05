using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleCollider : MonoBehaviour
{
    private Vector3 prevPos; //previous recorded position of collieder
    private Vector3 prevEularAngle;
    private Vector3 velocity; //velocity of collider
    private Vector3 angularVelocity;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = this.transform.position;
        prevEularAngle = this.transform.eulerAngles;
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        velocity = (this.transform.position - prevPos) / Time.fixedDeltaTime;
        prevPos = this.transform.position;
        angularVelocity = (this.transform.eulerAngles - prevEularAngle) / Time.fixedDeltaTime;
        prevEularAngle = this.transform.eulerAngles;
    }

    public Vector3 CurrentVelocity()
    {
        return velocity;
    }

    public Vector3 CurrentAngularVelocity()
    {
        return angularVelocity;
    }
}
