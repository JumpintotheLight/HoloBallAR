using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PongPlayerController : NetworkBehaviour
{
    private MouseLook mouseLook;

    [SyncVar]
    private int playerNum = 0;
    private int pointsLost = 0;


    public override void OnStartLocalPlayer()
    {
        GetComponent<Renderer>().material.color = Color.blue;

        if (isLocalPlayer)
        {
            // attach camera to player.. 3rd person view..
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(0, 1.33f, -0.69f);
            Camera.main.transform.localRotation = Quaternion.Euler(6.31f, 0, 0);

            mouseLook = new MouseLook();
            mouseLook.Init(transform, Camera.main.transform);
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
            CmdStartBall(playerNum);
        }

        mouseLook.LookRotation(transform, Camera.main.transform);

        transform.rotation = Camera.main.transform.rotation;
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
        playerNum = Mirror3DPongGameDriver.gameDriver.PlayerClockIn(netId, false);
        /*foreach (GameObject g in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (g.GetComponent<PlayerGoal>().GoalNumber == playerNum)
            {
                g.GetComponent<PlayerGoal>().BindToPlayer(netId, false);
            }
        }*/
    }

    [Command]
    void CmdStartBall(int pNum)
    {
        Mirror3DPongGameDriver.gameDriver.StartBall(pNum);
    }
}
