using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerGoal : NetworkBehaviour
{
    [SerializeField]
    private int goalNum;
    private uint localPlayerId;
    private bool playerIsVR = false;

    public int GoalNumber
    {
        get { return goalNum; }
        set { goalNum = value; }
    }

    public void BindToPlayer(uint playerId, bool isVR)
    {
        localPlayerId = playerId;
        playerIsVR = isVR;
        Debug.Log("Player " + goalNum + " goal says his ID is: " + localPlayerId);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            Destroy(collision.gameObject);
            if (isServer)
            {
                Mirror3DPongGameDriver.gameDriver.ScorePoint(goalNum);
            }

            /*if (playerIsVR)
            {
                NetworkIdentity.spawned[localPlayerId].gameObject.GetComponent<VRPongPlayerController>().OpponentScored();
            }
            else
            {
                NetworkIdentity.spawned[localPlayerId].gameObject.GetComponent<PongPlayerController>().OpponentScored();
            }*/

            //CmdScorePoint(goalNum);
        }
    }

    /*[Command]
    public void CmdScorePoint(int gN)
    {
        Mirror3DPongGameDriver.gameDriver.ScorePoint(gN);
    }*/
}
