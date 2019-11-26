using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Mirror3DPongGameDriver : NetworkBehaviour
{
    public static Mirror3DPongGameDriver gameDriver;

    private int priorityPlayer = 1;
    private int p1Score = 0;
    private int p1Wins = 0;
    private int p2Score = 0;
    private int p2Wins = 0;
    private bool ballMoving = false;
    [SerializeField]
    private int winPoint = 12;
    [SerializeField]
    private int advantageNeeded = 0;
    //Temporary field for testing; timer for automaticaly launching ball
    private float ballStartTime = 3f;
    private float ballStartTimer = 0;

    [SerializeField]
    private GameObject ballPrefab;
    private Transform ballSpawnPoint;

    
    public Text p1ScoreText;

    public Text p1Wintext;
    
    public Text p2ScoreText;

    public Text p2Wintext;

    private int playerCount = 0;
    private bool gameWon = false;

    private void Awake()
    {
        if(gameDriver != null)
        {
            if(gameDriver != this)
            {
                Debug.Log("GD Destroyed");
                GameObject.Destroy(this.gameObject);
            }
        }
        gameDriver = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public void SetBallInfo(GameObject b, Transform bSP)
    {
        ballPrefab = b;
        ballSpawnPoint = bSP;
    }

    public int WinPoint
    {
        get { return winPoint; }
        set
        {
            if (value >= 5)
            {
                winPoint = value;
            }
        }
    }

    public int AdvantageNeeded
    {
        get { return advantageNeeded; }
        set
        {
            if (value >= 0)
            {
                advantageNeeded = value;
            }
        }
    }

    private void Update()
    {
        if(isServer && playerCount == 2)
        {
            if (!ballMoving)
            {
                ballStartTimer += Time.deltaTime;
                if (ballStartTimer >= ballStartTime)
                {
                    ballStartTimer = 0f;
                    StartBall(priorityPlayer);
                }
            }
        }
        
    }

    public void StartBall(int playerNum)
    {
        if(isServer && !ballMoving)
        {
            if (playerNum == priorityPlayer)
            {
                ballStartTimer = 0;
                float newX = Random.Range(-2f, 2f);
                float newY = Random.Range(-1f, 1f);
                float newZ = Random.Range(6f, 8f);
                newZ *= priorityPlayer == 1 ? 1 : -1;
                Vector3 ballVelocity = new Vector3(newX, newY, newZ);
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
        Debug.Log("Scoring Point");
        Debug.Log("IsServer " + isServer);
        if (goalNumber == 1)
        {
            priorityPlayer = 1;
            p2Score++;
            if(p2Score == winPoint && p2Score - p1Score >= advantageNeeded)
            {
                p2Wins++;
                gameWon = true;
            }
            //CmdSetScoreText(2, p2Score);
            //p2Score++;
        }
        else
        {
            priorityPlayer = 2;
            p1Score++;
            if (p1Score == winPoint && p1Score - p2Score >= advantageNeeded)
            {
                p1Wins++;
                gameWon = true;
            }
            //CmdSetScoreText(1, p1Score);
            //RpcSetScoreText(p1Score, p2Score);
            //p1Score++;
        }
        if (gameWon)
        {
            Debug.Log("Game Won!");
            RpcSetWinText(p1Wins, p2Wins);
            Reset();
        }
        else
        {
            RpcSetScoreText(p1Score, p2Score);
            ballMoving = false;
        }
        Debug.Log("Score: [P1: " + p1Score + "]  [P2: " + p2Score + "]");
        Debug.Log("Priority= " + priorityPlayer);
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

    public void PlayerDisconnected()
    {
        NetworkServer.Destroy(GameObject.FindObjectOfType<Ball3DMirror>().gameObject);
        playerCount--;
        HardReset();
    }

    public void Reset()
    {
        gameWon = false;
        priorityPlayer = 1;
        p1Score = 0;
        p2Score = 0;
        ballMoving = false;
        RpcSetScoreText(0, 0);
    }

    public void HardReset()
    {
        p1Wins = 0;
        p2Wins = 0;
        RpcSetWinText(0, 0);
        Reset();
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
        if (playerCount == 2)
        {
            SpawnBall();
        }
        return playerCount;
    }

    /*[Command]
    void CmdSetScoreText(int playerNum, int newScore)
    {
        RpcSetScoreText(playerNum, newScore);
    }*/

    [ClientRpc]
    void RpcSetScoreText(int newP1Score, int newP2Score)
    {
        if (isClient)
        {
            Debug.Log("Set SCore RPC called");
            /*if (playerNum == 1)
            {
                p1ScoreText.text = "" + newScore;
            }
            else
            {
                p2ScoreText.text = "" + newScore;
            }*/
            if(newP1Score == winPoint-1 && newP1Score-newP2Score >= advantageNeeded -1)
            {
                p1ScoreText.text = "GP";
            }
            else
            {
                p1ScoreText.text = "" + newP1Score;
            }
            if (newP2Score == winPoint - 1 && newP2Score - newP1Score >= advantageNeeded -1)
            {
                p2ScoreText.text = "GP";
            }
            else
            {
                p2ScoreText.text = "" + newP2Score;
            }
        }
        
    }

    [ClientRpc]
    void RpcSetWinText(int newP1Win, int newP2Win)
    {
        if (isClient)
        {
            Debug.Log("Set Wins RPC called");
            p1Wintext.text = "Wins: " + newP1Win;
            p2Wintext.text = "Wins: " + newP2Win;
        }

    }
}
