using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public enum PongGameState
{
    Setup,
    Standby,
    Playing,
    Paused,
    Won
}

public class Mirror3DPongGameDriver : NetworkBehaviour
{
    public static Mirror3DPongGameDriver gameDriver;

    private int priorityPlayer = 1;
    private uint p1NetID;
    private uint p2NetID;
    private int p1Score = 0;
    private int p1Wins = 0;
    private int p2Score = 0;
    private int p2Wins = 0;
    private PongGameState gameState = PongGameState.Setup;
    private PongGameState pausedState;
    [SerializeField] [SyncVar]
    private int winPoint = 5;
    [SerializeField] [SyncVar]
    private int advantageNeeded = 0;
    //Ball launch timer
    private float ballStartTime = 3f;
    private float ballStartTimer = 3f;
    //Victory state timer
    private float victoryTime = 4f;
    private float victoryTimer = 4f;
    //Bool for starting new game without UIManager inut
    private bool continuousPlay = false;



    [SerializeField]
    private GameObject ballPrefab;
    private Transform ballSpawnPoint;
    private GameObject currentBall;

    //fields for boundaries
    //private GameObject rightBoundary;
    //private GameObject leftBoundary;
    private GameObject topBoundary;
    [SerializeField]
    private float boundaryWidth = 2.526668f;
    [SerializeField]
    private float boundaryHeight = 1.514f;
    [SerializeField]
    private float gameSpaceAngle = 349f;
    [SerializeField]
    private bool boundariesActive = true;

    //Goals, Player spawns, game space container, and goal scale without boundaries
    //[SerializeField]
    ///private float unboundGoalWidth;
    //[SerializeField]
    //private float unboundGoalHeight;
    private GameObject gameSpaceContainer;
    //private GameObject p1Goal;
    //private GameObject p2Goal;
    private GameObject p1Start;
    private GameObject p2Start;


    //AI Player Fields
    [SerializeField]
    private GameObject aiPlayerPrefab;
    private GameObject aiPlayer;
    [SerializeField]
    private Vector2 aiSpeedFactor;
    [SerializeField]
    private float aiErrorXFactor;
    [SerializeField]
    private float aiErrorYFactor;
    [SerializeField]
    private int aiLevel = 1;
    private Transform aiStartPos;
    private bool aiPlayerActive = false;



    //UI Elements
    public Text p1ScoreText;
    public Text p1Wintext;
    public Text p2ScoreText;
    public Text p2Wintext;
    public Text victoryMessageText;
    public Text countdownText;

    private int playerCount = 0;
    private int minPlayerCount = 2;
    //private bool gameWon = false;

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

    private void Start()
    {
        //Find and set Boundaries
        //rightBoundary = GameObject.Find("RightBoundary");
        //leftBoundary = GameObject.Find("LeftBoundary");
        gameSpaceContainer = GameObject.Find("GameSpace");
        topBoundary = GameObject.Find("TopBoundary");
        //p1Goal = GameObject.Find("Player1Goal");
        //p2Goal = GameObject.Find("Player2Goal");
        aiStartPos = GameObject.Find("AISpawnPoint").transform;
        BoundaryWidth = boundaryWidth;
        BoundaryHeight = boundaryHeight;
        GameSpaceAngle = gameSpaceAngle;
        BoundariesActive = boundariesActive;
    }

    public PongGameState GameState
    {
        get { return gameState; }
    }

    public int PlayerCount
    {
        get { return playerCount; }
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
            winPoint = (int)Mathf.Clamp(value, 5, 20);
        }
    }

    public int AdvantageNeeded
    {
        get { return advantageNeeded; }
        set
        {
            advantageNeeded = (int)Mathf.Clamp(value, 0, 3);
        }
    }

    public float BoundaryWidth
    {
        get { return boundaryWidth; }
        set
        {
            boundaryWidth = Mathf.Clamp(value,1f,5f);
            topBoundary.transform.localScale = new Vector3(topBoundary.transform.localScale.x, boundaryWidth, topBoundary.transform.localScale.z);
            //rightBoundary.transform.localPosition = new Vector3(rightBoundary.transform.localPosition.x, boundaryWidth / -2, rightBoundary.transform.localPosition.z);
            //leftBoundary.transform.localPosition = new Vector3(leftBoundary.transform.localPosition.x, boundaryWidth / 2, leftBoundary.transform.localPosition.z);

            //Scale goals
            /*ScaleGoalWidth(p1Goal);
            ScaleGoalWidth(p2Goal);*/
        }
    }

    public float BoundaryHeight
    {
        get { return boundaryHeight; }
        set
        {
            boundaryHeight = Mathf.Clamp(value, 0.8f, 3f);
            topBoundary.transform.position = new Vector3(topBoundary.transform.position.x, boundaryHeight, topBoundary.transform.position.z);
            /*ScaleGoalHeight(p1Goal);
            ScaleGoalWidth(p2Goal);*/
        }
    }

    public float GameSpaceAngle
    {
        get { return gameSpaceAngle; }
        set
        {
            gameSpaceAngle = Mathf.Clamp(value, -360f, 360f);
            gameSpaceContainer.transform.localEulerAngles = new Vector3(gameSpaceContainer.transform.localEulerAngles.x, gameSpaceAngle, gameSpaceContainer.transform.localEulerAngles.z);
        }
    }

    public bool BoundariesActive
    {
        get { return boundariesActive; }
        set
        {
            boundariesActive = value;
            topBoundary.SetActive(boundariesActive);
            if (isServer)
            {
                RpcEnableBoundaries(boundariesActive);
            }
            /*if (boundariesActive)
            {
                ScaleGoalWidth(p1Goal);
                ScaleGoalWidth(p2Goal);
                ScaleGoalHeight(p1Goal);
                ScaleGoalHeight(p2Goal);
            }
            else
            {
                ScaleGoalUnbound(p1Goal);
                ScaleGoalUnbound(p2Goal);
            }*/
            //rightBoundary.SetActive(boundariesActive);
            //leftBoundary.SetActive(boundariesActive);
        }
    }

    //Methods for scaling goal sizes
   /* private void ScaleGoalWidth(GameObject goal)
    {
        float currentWidth = goal.GetComponent<Collider>().bounds.size.z;
        float targetWidth = topBoundary.GetComponent<Collider>().bounds.size.y;
        goal.transform.localScale = new Vector3(goal.transform.localScale.x, goal.transform.localScale.y, (targetWidth * goal.transform.localScale.z / currentWidth) + 1);
    }

    private void ScaleGoalHeight(GameObject goal)
    {
        float currentHeight = goal.GetComponent<Collider>().bounds.size.y;
        float targetHeight = topBoundary.transform.position.y;
        goal.transform.localScale = new Vector3(goal.transform.localScale.x, (targetHeight * goal.transform.localScale.y / currentHeight) + 1, goal.transform.localScale.z);
    }

    private void ScaleGoalUnbound(GameObject goal)
    {
        goal.transform.localScale = new Vector3(goal.transform.localScale.x, unboundGoalHeight, unboundGoalWidth);
    }*/

    public bool ContinuousPlay
    {
        get { return continuousPlay; }
        set { continuousPlay = value; }
    }

    public bool AiPlayerActive
    {
        get { return aiPlayerActive; }
        set
        {
            aiPlayerActive = value;
            if (aiPlayerActive)
            {
                minPlayerCount = 1;
            }
            else
            {
                minPlayerCount = 2;
            }
        }
    }

    public int AiLevel
    {
        get { return aiLevel; }
        set
        {
            aiLevel = (int)Mathf.Clamp(value, 1, 5);
        }
    }

    public Vector3 CurrentBallPosition
    {
        get
        {
            if(currentBall != null)
            {
                return currentBall.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    public bool BallHeadingTowardsAI
    {
        get
        {
            return currentBall.GetComponent<Rigidbody>().velocity.z > 0;
        }
    }




    private void Update()
    {
        if(isServer)
        {
            if (gameState == PongGameState.Standby)
            {
                ballStartTimer -= Time.deltaTime;
                if (ballStartTimer <= 0)
                {
                    RpcSetCountdownText("");
                    ballStartTimer = ballStartTime;
                    StartBall();
                }
                else
                {
                    int cd = (int)Mathf.Ceil(ballStartTimer);
                    RpcSetCountdownText(cd.ToString());
                }
            }
            else if(gameState == PongGameState.Won)
            {
                victoryTimer -= Time.deltaTime;
                if(victoryTimer <= 0)
                {
                    victoryTimer = victoryTime;
                    Reset();
                }
            }
        }
        
    }

    public void StartGame()
    {
        if (isServer)
        {
            if(gameState == PongGameState.Setup && playerCount == minPlayerCount)
            {
                if (aiPlayerActive)
                {
                    SpawnAIPlayer();
                }
                SpawnBall();
                ballStartTimer = ballStartTime;
                gameState = PongGameState.Standby;
            }
        }
        
    }

    public void StartBall()
    {
        if(isServer && gameState==PongGameState.Standby)
        {
            
                ballStartTimer = ballStartTime;
                gameState = PongGameState.Playing;
                float newX = Random.Range(-2f, 2f);
                float newY = Random.Range(-1f, 1f);
                float newZ = Random.Range(3f, 4f);
                newZ *= priorityPlayer == 1 ? 1 : -1;
                Vector3 ballVelocity = new Vector3(newX, newY, newZ);
                GameObject.FindObjectOfType<Ball3DMirror>().StartBall(ballVelocity);

                /*GameObject ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);
                Vector3 ballVelocity = priorityPlayer == 1 ? new Vector3(2f, 0f, 8f) : new Vector3(-2f, 0f, -8f);
                NetworkServer.Spawn(ball);
                ball.GetComponent<Ball3DMirror>().StartBall(ballVelocity);*/
            
        }
    }

    public void ScorePoint(int goalNumber)
    {
        Debug.Log("Scoring Point");
        Debug.Log("IsServer " + isServer);
        //gameState = PongGameState.Standby;
        if (goalNumber == 1)
        {
            priorityPlayer = 1;
            p2Score++;
            if(p2Score == winPoint && p2Score - p1Score >= advantageNeeded)
            {
                p2Wins++;
                gameState = PongGameState.Won;
                //gameWon = true;
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
                gameState = PongGameState.Won;
                //gameWon = true;
            }
            //CmdSetScoreText(1, p1Score);
            //RpcSetScoreText(p1Score, p2Score);
            //p1Score++;
        }
        if (gameState == PongGameState.Won)
        {
            Debug.Log("Game Won!");
            SetWinText(p1Wins, p2Wins);
            RpcSetWinText(p1Wins, p2Wins);
            RpcSetVictoryText(3 - goalNumber == 1 ? p1NetID : p2NetID);
        }
        else
        {
            SetScoreText(p1Score, p2Score);
            RpcSetScoreText(p1Score, p2Score);
            SpawnBall();
            gameState = PongGameState.Standby;
        }
        Debug.Log("Score: [P1: " + p1Score + "]  [P2: " + p2Score + "]");
        Debug.Log("Priority= " + priorityPlayer);
        

    }

    private void SetScoreText(int newP1Score, int newP2Score)
    {
        if (newP1Score == winPoint - 1 && newP1Score - newP2Score >= advantageNeeded - 1)
        {
            p1ScoreText.text = "GP";
        }
        else
        {
            p1ScoreText.text = "" + newP1Score;
        }
        if (newP2Score == winPoint - 1 && newP2Score - newP1Score >= advantageNeeded - 1)
        {
            p2ScoreText.text = "GP";
        }
        else
        {
            p2ScoreText.text = "" + newP2Score;
        }
    }

    private void SetWinText(int newP1Win, int newP2Win)
    {
        p1Wintext.text = "Wins: " + newP1Win;
        p2Wintext.text = "Wins: " + newP2Win;
    }

    private void SetVictoryText(uint winnerID)
    {
        Debug.Log("Winner targetID: " + winnerID);
        foreach (GameObject pl in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (pl.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                Debug.Log("Found Player NetID: " + pl.GetComponent<NetworkIdentity>().netId);
                
                if(pl.GetComponent<NetworkIdentity>().netId == winnerID)
                {
                    victoryMessageText.text = "You Win!!";
                    
                }
                else
                {
                    victoryMessageText.text = "You Lose...";
                }
            }
        }
    }

    private void SetCountdownText(string newText)
    {
        countdownText.text = newText;
    }


    private void ClearVictoryText()
    {
        victoryMessageText.text = "";
    }

    public void SetClientTextRotation(int playerNum)
    {
        //
        float newEularY = 0f;
        if (playerNum == 1)
        {
            newEularY = 90f;
        }
        else
        {
            newEularY = -90f;
        }

        victoryMessageText.rectTransform.localEulerAngles = new Vector3(0f, newEularY, 0f);
        countdownText.rectTransform.localEulerAngles = new Vector3(0f, newEularY, 0f);
    }

    private void SpawnBall()
    {
        GameObject ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        currentBall = ball;
        NetworkServer.Spawn(ball);
        if (aiPlayerActive)
        {
            Debug.Log("AI player is null? " + aiPlayer == null);
            Debug.Log("Ball is null? " + currentBall == null);
        }
    }

    private void SpawnAIPlayer()
    {
        GameObject aiPlayer = (GameObject)Instantiate(aiPlayerPrefab, aiStartPos.position, aiStartPos.rotation);
        AIPongPlayer aiCom = aiPlayer.GetComponent<AIPongPlayer>();
        aiCom.speed += aiSpeedFactor * (aiLevel - 1);
        aiCom.errorX = Mathf.Max(0f, aiCom.errorX - (aiErrorXFactor * (aiLevel -1)));
        aiCom.errorY = Mathf.Max(0f, aiCom.errorY - (aiErrorYFactor * (aiLevel - 1)));
        NetworkServer.Spawn(aiPlayer);
    }

    public void PlayerDisconnected()
    {
        NetworkServer.Destroy(GameObject.FindObjectOfType<Ball3DMirror>().gameObject);
        playerCount--;
        HardReset();
    }

    public void Reset()
    {
        if(gameState == PongGameState.Paused)
        {
            Time.timeScale = 1f;
            RpcSetTimeScale(1f);
        }
        if(currentBall != null)
        {
            NetworkServer.Destroy(currentBall);
        }
        if (aiPlayerActive)
        {
            NetworkServer.Destroy(aiPlayer);
            playerCount--;
        }
        gameState = PongGameState.Setup;
        priorityPlayer = 1;
        p1Score = 0;
        p2Score = 0;
        SetScoreText(0, 0);
        RpcSetScoreText(0, 0);
        RpcSetCountdownText("");
        RpcClearVictoryText();
        if(continuousPlay)
        {
            StartGame();
        }
    }

    public void HardReset()
    {
        p1Wins = 0;
        p2Wins = 0;
        SetWinText(0, 0);
        RpcSetWinText(0, 0);
        Reset();
    }

    public void PauseGame()
    {
        if(gameState != PongGameState.Setup)
        {
            pausedState = gameState;
            RpcSetCountdownText("Paused");
            gameState = PongGameState.Paused;
            Time.timeScale = 0f;
            RpcSetTimeScale(0f);
        }
    }

    public void UnPauseGame()
    {
        if(gameState == PongGameState.Paused)
        {
            Time.timeScale = 1f;
            RpcSetTimeScale(1f);
            RpcSetCountdownText("");
            gameState = pausedState;
        }
    }

    public int PlayerClockIn(uint pId, bool isVR, NetworkConnection conn)
    {
        playerCount++;
        if (playerCount == 1)
        {
            p1NetID = pId;
        }
        else
        {
            p2NetID = pId;
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Goal"))
        {
            if (g.GetComponent<PlayerGoal>().GoalNumber == playerCount)
            {
                g.GetComponent<PlayerGoal>().BindToPlayer(pId, isVR);
            }
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
            SetScoreText(newP1Score, newP2Score);
            Debug.Log("Set SCore RPC called");
            /*if (playerNum == 1)
            {
                p1ScoreText.text = "" + newScore;
            }
            else
            {
                p2ScoreText.text = "" + newScore;
            }*/
        }
        
    }

    [ClientRpc]
    void RpcSetWinText(int newP1Win, int newP2Win)
    {
        if (isClient)
        {
            Debug.Log("Set Wins RPC called");
            SetWinText(newP1Win, newP2Win);
        }

    }

    [ClientRpc]
    void RpcSetVictoryText(uint winnerId)
    {
        if (isClient)
        {
            SetVictoryText(winnerId);
        }
    }

    [ClientRpc]
    void RpcClearVictoryText()
    {
        if (isClient)
        {
            ClearVictoryText();
        }
    }

    [ClientRpc]
    void RpcSetCountdownText(string newString)
    {
        if(isClient)
        {
            SetCountdownText(newString);
        }
    }


    [ClientRpc]
    void RpcEnableBoundaries(bool enable)
    {
        topBoundary.SetActive(enable);
    }

    [ClientRpc]
    void RpcSetTimeScale(float newTime)
    {
        Time.timeScale = newTime;
    }
}
