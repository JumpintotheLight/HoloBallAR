using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AIPongPlayer : NetworkBehaviour
{
    [SyncVar]
    private int playerNum = 0;

    [SerializeField]
    private Vector2 baseSpeed; //Base AI speed per second
    [SerializeField]
    private float baseErrorX; //base x position error range (doubled in code)
    [SerializeField]
    private float baseErrorY; //base y position error range (doubled in code)

    public Vector2 speed;
    public float errorX;
    public float errorY;

    //public GameObject targetBall;

    // Start is called before the first frame update
    void Start()
    {
        speed = baseSpeed;
        errorX = baseErrorX;
        errorY = baseErrorY;
        CmdPlayerClockIn();
    }


    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            if (Mirror3DPongGameDriver.gameDriver.GameState == PongGameState.Playing)
            {
                if(Mirror3DPongGameDriver.gameDriver.BallHeadingTowardsAI)
                {
                    float targetX = Random.Range(Mirror3DPongGameDriver.gameDriver.CurrentBallPosition.x - errorX, Mirror3DPongGameDriver.gameDriver.CurrentBallPosition.x + errorX);
                    float targetY = Random.Range(Mirror3DPongGameDriver.gameDriver.CurrentBallPosition.y - errorY, Mirror3DPongGameDriver.gameDriver.CurrentBallPosition.y + errorY);
                    float newX = Mathf.MoveTowards(this.transform.position.x, targetX, speed.x * Time.deltaTime);
                    float newY = Mathf.MoveTowards(this.transform.position.y, targetY, speed.y * Time.deltaTime);
                    this.transform.position = new Vector3(newX, newY, this.transform.position.z);
                }
            }
        }
    }

    [Command]
    void CmdPlayerClockIn()
    {
        playerNum = Mirror3DPongGameDriver.gameDriver.PlayerClockIn(netId, false, connectionToClient);
    }
}
