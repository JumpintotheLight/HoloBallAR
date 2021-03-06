﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PongPlayerController : NetworkBehaviour
{
    //private MouseLook mouseLook;

    [SyncVar]
    private int playerNum = 0;
    private int pointsLost = 0;

    // Store reference to camera since we use it in multiple places.
    private Camera camera;

    public override void OnStartLocalPlayer()
    {
        GetComponent<Renderer>().material.color = Color.blue;

        if (isLocalPlayer)
        {
            // attach camera to player.. 3rd person view..
            camera = Camera.main;
            camera.transform.parent = transform;
            camera.transform.localPosition = new Vector3(0, 1.33f, -0.69f);
            camera.transform.localRotation = Quaternion.Euler(6.31f, 0, 0);

            //mouseLook = new MouseLook();
            //mouseLook.Init(transform, camera.transform);
            Debug.Log("NVR Players Says his ID is: " + netId);
            CmdPlayerClockIn();
        }
        
    }

    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }


        // non vr player input here
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Translate(x, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdPauseGame();
        }

        //mouseLook.LookRotation(transform, camera.transform);

        //transform.rotation = camera.transform.rotation;
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
        playerNum = Mirror3DPongGameDriver.gameDriver.PlayerClockIn(netId, false, connectionToClient);
        TargetSetClientTextRotation(connectionToClient, playerNum);
        /*foreach (GameObject g in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (g.GetComponent<PlayerGoal>().GoalNumber == playerNum)
            {
                g.GetComponent<PlayerGoal>().BindToPlayer(netId, false);
            }
        }*/
    }

    [TargetRpc]
    void TargetSetClientTextRotation(NetworkConnection target, int pN)
    {
        Mirror3DPongGameDriver.gameDriver.SetClientTextRotation(pN);
    }

    [Command]
    void CmdPauseGame()
    {
        if(Mirror3DPongGameDriver.gameDriver.GameState == PongGameState.Paused)
        {
            Mirror3DPongGameDriver.gameDriver.UnPauseGame();
        }
        else
        {
            Mirror3DPongGameDriver.gameDriver.PauseGame();
        }
    }

    /* [Command]
     void CmdStartBall(int pNum)
     {
         Mirror3DPongGameDriver.gameDriver.StartBall(pNum);
     }*/
}
