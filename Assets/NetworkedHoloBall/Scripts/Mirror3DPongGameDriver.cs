using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Mirror3DPongGameDriver : NetworkBehaviour
{
    public static Mirror3DPongGameDriver gameDriver;

    private int priorityPlayer = 1;
    [SyncVar]
    private int p1Score = 0;
    [SyncVar]
    private int p2Score = 0;
    private bool ballMoving = false;

    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private Transform ballSpawnPoint;

    
    public Text p1ScoreText;
    
    public Text p2ScoreText;

    private int playerCount = 0;

    private void Awake()
    {
        if(gameDriver != null)
        {
            if(gameDriver != this)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
        gameDriver = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public void StartBall(int playerNum)
    {
        if(isServer && !ballMoving)
        {
            if (playerNum == priorityPlayer)
            {
                Vector3 ballVelocity = priorityPlayer == 1 ? new Vector3(2f, 0f, 8f) : new Vector3(-2f, 0f, -8f);
                GameObject.FindObjectOfType<Ball3DMirror>().StartBall(ballVelocity);

                /*GameObject ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);
                Vector3 ballVelocity = priorityPlayer == 1 ? new Vector3(2f, 0f, 8f) : new Vector3(-2f, 0f, -8f);
                NetworkServer.Spawn(ball);
                ball.GetComponent<Ball3DMirror>().StartBall(ballVelocity);*/
                ballMoving = true;
            }
        }
    }

    public void ScorePoint(int goalNumber)
    {
        if(goalNumber == 1)
        {
            priorityPlayer = 1;
            RpcSetScoreText(2, p2Score + 1);
            p2Score++;
        }
        else
        {
            priorityPlayer = 2;
            RpcSetScoreText(1, p1Score + 1);
            p1Score++;
        }
        Debug.Log("Score: [P1: " + p1Score + "]  [P2: " + p2Score + "]");
        Debug.Log("Priority= " + priorityPlayer);
        ballMoving = false;
        SpawnBall();
    }

    /*private void OnP1Score(int newScore)
    {
        p1Score = newScore;
        RpcSetScoreText(1, newScore);
    }

    private void OnP2Score(int newScore)
    {
        p2Score = newScore;
        RpcSetScoreText(2, newScore);
    }*/

    private void SpawnBall()
    {
        GameObject ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        NetworkServer.Spawn(ball);
    }

    private void OnPlayerDisconnected(NetworkIdentity player)
    {
        ballMoving = false;
        playerCount--;
        Reset();
    }

    private void Reset()
    {
        priorityPlayer = 1;
        p1Score = 0;
        p2Score = 0;
        ballMoving = false;
        p1ScoreText.text = "0";
        p2ScoreText.text = "0";
    }

    public int PlayerClockIn(uint pId, bool isVR)
    {
        playerCount++;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (g.GetComponent<PlayerGoal>().GoalNumber == playerCount)
            {
                g.GetComponent<PlayerGoal>().BindToPlayer(pId, isVR);
            }
        }
        return playerCount;
    }

    [ClientRpc]
    void RpcSetScoreText(int playerNum, int newScore)
    {
        Debug.Log("Set SCore RPC called");
        if(playerNum == 1)
        {
            p1ScoreText.text = "" + newScore;
        }
        else
        {
            p2ScoreText.text = "" + newScore;
        }
    }
}
