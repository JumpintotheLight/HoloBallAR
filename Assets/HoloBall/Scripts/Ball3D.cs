using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;


public class Ball3D : MonoBehaviour{
    private Paddle3D paddle;
    private Vector3 paddleToBallVector;

   // public SteamVR_TrackedObject controller;


    public bool triggerButtonDown = false;
    private bool hasStarted = false;


    /*
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller
    {

        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);

        }

    }
    */




    // Use this for initialization
    void Start () {
        paddle = GameObject.FindObjectOfType<Paddle3D>();
        if(paddle !=null)
        {
            //paddleToBallVector = this.transform.position - paddle.transform.position;
        }
        else
        {
            Debug.Log("Paddle is null.");
        }
        //trackedObj = GetComponent();
        //trackedObj.GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!hasStarted)
        {
            // Lock the ball relative to paddle
            //this.transform.position = paddle.transform.position + paddleToBallVector;

            //Starts game and gives ball velocity...waits for mouse press
            // FIXME
            /*
            if (Input.GetMouseButtonDown(0))
            {
                hasStarted = true;
                this.GetComponent<Rigidbody>().velocity = new Vector3(2f, 0f, 8f);
            }
            

            if (controller == null)
            {

                Debug.Log("Controller not initialized");

                return;

            }
            */
            

            triggerButtonDown = ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger);

            if (triggerButtonDown)
            {

                Debug.Log("Fire");
                hasStarted = true;
                this.GetComponent<Rigidbody>().velocity = new Vector3(2f, 0f, 8f);

            }

        }
	}

    

    void OnCollisionEnter(Collision collision) {
        //Vector3 tweak = new Vector3(Random.Range(0f, 0.2f), Random.Range(0f, 0.1f), Random.Range(0f, 0.1f));
        //this.GetComponent<Rigidbody>().velocity += tweak;
    }
}
