using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    [HideInInspector]
    public bool gameEnded = false; // is the game done?
    public float timeToWin; // amount of time the player needs to win
    public float invincibleDuration; // iframes for the player that got a hat
    private float hatPickupTime; // how long the hat has been picked up by the current holder

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;

    //[Header("Components")]
    //public PhotonView photonView;

    //instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        // when all the players load in the scene, spawn them
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    // spawn a player and initialize them
    void SpawnPlayer()
    {
        // instantiate the player through the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // get the player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        // initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    // returns the player who has the requested id
    public PlayerController GetPlayer (int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    // returns the player of the requested GameObject
    public PlayerController GetPlayer (GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    // called when a player hits the hatted player, which transfers the hat
    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        // remove the hat from the current hat player
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        // give the hat to the new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }

    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
