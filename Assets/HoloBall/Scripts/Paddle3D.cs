using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Paddle3D : MonoBehaviour {


    public SteamVR_TrackedObject controller;
    private Vector3 paddlePos;
    private Quaternion paddleRot;

    // Use this for initialization
    void Start () {
         paddlePos = controller.transform.position;
         //paddleRot = controller.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {


        if (controller != null)
        {

            transform.position = controller.transform.position;
            //transform.rotation = controller.transform.rotation;

        }

        /*
        // FIXME

        //float mousePosBlocks = Input.mousePosition.x / Screen.width * 16;

        paddlePos.x = Mathf.Clamp(paddlePos.x, -1.5f,1.5f);
        paddleRot = Quaternion.identity;
        this.transform.position = paddlePos;
        this.transform.rotation = paddleRot;
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        ControllerCollider collider = other.GetComponent<ControllerCollider>();
        if (collider != null)
        {
            controller = collider.controller;
        }

    }
}
