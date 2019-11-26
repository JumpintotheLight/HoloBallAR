using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VRPongPlayerController : NetworkBehaviour
{
    public GameObject vrCameraRig;
    //public GameObject leftHandPrefab;
    //public GameObject rightHandPrefab;
    public GameObject paddlePrefab;
    private GameObject vrCameraRigInstance;

    private int pointsLost = 0;

    [SyncVar]
    private int playerNum = 0;

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Debug.Log("OSLP on VR going through.");
        // delete main camera
        DestroyImmediate(Camera.main.gameObject);

        // create camera rig and attach player model to it
        vrCameraRigInstance = (GameObject)Instantiate(
            vrCameraRig,
            transform.position,
            transform.rotation);

        Transform bodyOfVrPlayer = transform.Find("VRPlayerBody");
        if (bodyOfVrPlayer != null) { bodyOfVrPlayer.parent = null; }

        GameObject head = vrCameraRigInstance.GetComponentInChildren<SteamVR_Camera>().gameObject;

        gameObject.GetComponent<F_CopyXForms>().target = head.transform;

        //transform.parent = head.transform;
        //transform.localPosition = new Vector3(0f, -0.03f, -0.06f);

        Debug.Log("VR Players Says his ID is: " + netId);
        CmdPlayerClockIn();
        TryDetectControllers();
    }


    public int PlayerNum
    {
        get { return playerNum; }
    }

    void TryDetectControllers()
    {
        var controllers = vrCameraRigInstance.GetComponentsInChildren<SteamVR_TrackedObject>();
        if (controllers != null && controllers.Length == 2 && controllers[0] != null && controllers[1] != null)
        {
            CmdSpawnPaddle(netId);
        }
        else
        {
            Invoke("TryDetectControllers", 2f);
        }
    }

    public void OpponentScored()
    {
        Debug.Log("Opponent Scored");
        pointsLost++;
        CmdScorePoint(playerNum);
    }

    [Command]
    void CmdScorePoint(int goalNum)
    {
        Mirror3DPongGameDriver.gameDriver.ScorePoint(goalNum);
    }

    [Command]
    void CmdPlayerClockIn()
    {
        playerNum = Mirror3DPongGameDriver.gameDriver.PlayerClockIn(netId, true);
        /*foreach (GameObject g in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if(g.GetComponent<PlayerGoal>().GoalNumber == playerNum)
            {
                g.GetComponent<PlayerGoal>().BindToPlayer(netId, true);
            }
        }*/
    }

    [Command]
    void CmdSpawnPaddle(uint playerId)
    {
        // instantiate controllers
        // tell the server, to spawn two new networked controller model prefabs on all clients
        // give the local player authority over the newly created controller models
        GameObject paddle = Instantiate(paddlePrefab);

        var paddleVR = paddle.GetComponent<Paddle3DMirror>();



        paddleVR.side = HandSide.Right;
        paddleVR.ownerId = playerId;

        NetworkServer.SpawnWithClientAuthority(paddle, base.connectionToClient);
    }


    /*// Called on Client, executed on Server. 
    [Command]
    public void CmdGrab(uint objectId, uint controllerId)
    {
        var iObject = NetworkServer.FindLocalObject(objectId);
        var networkIdentity = iObject.GetComponent<NetworkIdentity>();
        networkIdentity.AssignClientAuthority(connectionToClient);

        var interactableObject = iObject.GetComponent<InteractableObject>();
        interactableObject.RpcAttachToHand(controllerId);    // client-side
        var hand = NetworkServer.FindLocalObject(controllerId);
        interactableObject.AttachToHand(hand);    // server-side
    }

    [Command]
    public void CmdDrop(uint objectId, Vector3 currentHolderVelocity)
    {
        var iObject = NetworkServer.FindLocalObject(objectId);
        var networkIdentity = iObject.GetComponent<NetworkIdentity>();
        networkIdentity.RemoveClientAuthority(connectionToClient);

        var interactableObject = iObject.GetComponent<InteractableObject>();
        interactableObject.RpcDetachFromHand(currentHolderVelocity); // client-side
        interactableObject.DetachFromHand(currentHolderVelocity); // server-side
    }*/
}
