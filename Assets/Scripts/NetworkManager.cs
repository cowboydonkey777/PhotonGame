using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //[Header("Components")]
    //public PhotonView photonView;

    // instance
    public static NetworkManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // attempts to create a room
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }
    /*
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }*/

    // attempt to join room
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // change scene using photon
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        if (sceneName.Equals("Menu"))
            PhotonNetwork.Destroy(gameObject);

        PhotonNetwork.LoadLevel(sceneName);
    }
}
