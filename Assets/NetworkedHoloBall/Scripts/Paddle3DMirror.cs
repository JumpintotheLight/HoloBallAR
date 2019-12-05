using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum HandSide
{
    Left,
    Right
}

public class Paddle3DMirror : NetworkBehaviour
{
    [SyncVar]
    public HandSide side;

    [SyncVar]
    public uint ownerId;

    //fields for position and rotation
    //private Transform targetHand;
    [SerializeField]
    private Vector3 positionOffset;
    [SerializeField]
    private Vector3 eularOffset;


    //Bools for controler inputs
    private bool hasTriggerBeenPressedThisFrame;
    private bool hasGripBeenPressedThisFrame;
    private bool hasTriggerBeenReleasedThisFrame;
    private bool hasGripBeenReleasedThisFrame;
    private bool isTriggerPressed;
    private bool isGripPressed;

    //public SteamVR_TrackedObject trackedController;
    private GameObject trackedController;
    private VRPongPlayerController localPlayer;
    private SteamVR_Controller.Device steamDevice;


    //fields for calculating velocity
    private Vector3 prevPos;
    private Vector3 currentVelocity;

    public override void OnStartAuthority()
    {
        // attach the controller model to the tracked controller object on the local client
        if (hasAuthority)
        {

            trackedController = GameObject.Find(string.Format("Controller ({0})", side.ToString("G").ToLowerInvariant()));

            //Helper.AttachAtGrip(trackedController.transform, transform);
            gameObject.GetComponent<F_CopyXForms>().target = trackedController.transform;
            //trackedController.gameObject.AddComponent<FixedJoint>();
            //trackedController.gameObject.GetComponent<FixedJoint>().connectedBody = this.gameObject.GetComponent<Rigidbody>();
            //UpdateTransform();
            //gameObject.GetComponent<FixedJoint>().connectedBody = trackedController.gameObject.GetComponent<Rigidbody>();

            localPlayer = NetworkIdentity.spawned[ownerId].GetComponent<VRPongPlayerController>();

            steamDevice = SteamVR_Controller.Input((int)trackedController.GetComponent<SteamVR_TrackedObject>().index);

        }
    }

    void QueryController()
    {
        hasGripBeenPressedThisFrame = false;
        hasTriggerBeenPressedThisFrame = false;
        hasTriggerBeenReleasedThisFrame = false;
        hasGripBeenReleasedThisFrame = false;
        if (steamDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {

            Debug.Log("GetTouchDown Trigger");
            if (!isTriggerPressed)
                hasTriggerBeenPressedThisFrame = true;

            isTriggerPressed = true;
        }
        else if (steamDevice.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("GetTouchUp Trigger");

            if (isTriggerPressed)
                hasTriggerBeenReleasedThisFrame = true;
            isTriggerPressed = false;
        }
        // Qucik Fix 
        //if (steamDevice.GetTouchDown (SteamVR_Controller.ButtonMask.Grip)) 
        if (steamDevice.GetPressDown(SteamVR_Controller.ButtonMask.Grip))

        {
            Debug.Log("GetTouchDown Grip");

            if (!isGripPressed)
                hasGripBeenPressedThisFrame = true;

            isGripPressed = true;
        }
        // Qucik Fix 
        //else if (steamDevice.GetTouchUp (SteamVR_Controller.ButtonMask.Grip)) 
        else if (steamDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("GetTouchUp Grip");

            if (isGripPressed)
                hasGripBeenReleasedThisFrame = true;
            isGripPressed = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateTransform();

        if (!hasAuthority)
        {
            return;
        }

        QueryController();

       
        /*if (hasTriggerBeenPressedThisFrame)
        {
            OnTriggerPressed();
        }*/
    }

    private void UpdateTransform()
    {
        this.transform.position = trackedController.transform.position + positionOffset;
        this.transform.eulerAngles = trackedController.transform.eulerAngles + eularOffset;
    }


    public Vector3 PaddleVelocity()
    {
        return steamDevice.velocity;
    }

    public Vector3 PaddleAngularVelocity()
    {
        return steamDevice.angularVelocity;
    }

    

    /*private void OnTriggerPressed()
    {
        CmdStartBall(localPlayer.PlayerNum);
    }

    [Command]
    private void CmdStartBall(int playerNum)
    {
        Mirror3DPongGameDriver.gameDriver.StartBall(playerNum);
    }*/
}
