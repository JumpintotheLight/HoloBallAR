using UnityEngine;
using Mirror;
using UnityEngine.XR;
using System.IO;
using System.Collections;

// Custom NetworkManager that simply assigns the correct racket positions when
// spawning players. The built in RoundRobin spawn method wouldn't work after
// someone reconnects (both players would be on the same side).
public class NetworkManager3DPong : NetworkManager
{

    public GameObject vrPlayerPrefab;
    //public bool forceNonVR = false;

    public Transform firstPlayerSpawn;
    public Transform secondPlayerSpawn;
    public Transform ballSpawnPoint;
    public GameObject ballPrefab;
    private GameObject ball;

    //Override OnStartServer to add handler for Player Message
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateVrPongPlayerMessage>(OnCreatePlayer);
    }

    //Override OnClientConnect to detect if the player is using VR
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        CreateVrPongPlayerMessage newPlayerMessage = new CreateVrPongPlayerMessage
        {
            isVrPlayer = UnityEngine.XR.XRDevice.isPresent
        };

        conn.Send(newPlayerMessage);
    }

    void OnCreatePlayer(NetworkConnection conn, CreateVrPongPlayerMessage message)
    {
        //Set corret start position
        Transform start = numPlayers == 0 ? this.firstPlayerSpawn : this.secondPlayerSpawn;

        GameObject newPlayer;
        if (message.isVrPlayer)
        {
            newPlayer = (GameObject)Instantiate(this.vrPlayerPrefab, start.position, start.rotation);
        }
        else
        {
            newPlayer = (GameObject)Instantiate(this.playerPrefab, start.position, start.rotation);
        }
        NetworkServer.AddPlayerForConnection(conn, newPlayer);

        if (numPlayers == 2)
        {
            //ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == ballPrefab.name), ballSpawnPoint.position, ballSpawnPoint.rotation);
            NetworkServer.Spawn(ball);
        }
    }

    /*public override void OnServerAddPlayer(NetworkConnection conn)
    {
        
        //Check if the player is a VR Player
        bool isVrPlayer = UnityEngine.XR.XRDevice.isPresent;

        Debug.Log("VR_True: " + isVrPlayer);

        //set correct spawn position
        Transform start = numPlayers == 0 ? this.firstPlayerSpawn : this.secondPlayerSpawn;

        //Setup new Player
        GameObject newPlayer;
        if (isVrPlayer)
        {
            newPlayer = (GameObject)Instantiate(this.vrPlayerPrefab, start.position, start.rotation);
        }
        else
        {
            newPlayer = (GameObject)Instantiate(this.playerPrefab, start.position + new Vector3(0,0.88f), start.rotation);
        }
        NetworkServer.AddPlayerForConnection(conn, newPlayer);

        // spawn ball if two players
       if (numPlayers == 2)
        {
            //ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == ballPrefab.name), ballSpawnPoint.position, ballSpawnPoint.rotation);
            NetworkServer.Spawn(ball);
        }
    }*/

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // destroy ball
        if (ball != null)
            NetworkServer.Destroy(ball);

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}

//MessageBase class for setting player type (Vr/Non-Vr)
public class CreateVrPongPlayerMessage : MessageBase
{
    public bool isVrPlayer;
}