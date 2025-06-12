//#define On

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public GameObject SpawnLocation;

    private void Awake()
    {
        if (FindObjectsOfType<PhotonManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        PhotonNetwork.NickName = "Guest";
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
#if On
        Debug.Log(PhotonNetwork.SendRate);
#endif
    }

    public override void OnConnectedToMaster()
    {
#if On
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
#endif
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
#if On
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
#endif
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
#if On
    Debug.Log($"JoinRoom Failed {returnCode}:{message}");
#endif
        RoomOptions room = new RoomOptions
        {
            MaxPlayers = 2,
            IsOpen = true,
            IsVisible = true
        };

        string uniqueRoomName = "Room_" + Random.Range(0, 10000);
        PhotonNetwork.CreateRoom(uniqueRoomName, room);
    }


    public override void OnCreatedRoom()
    {
#if On
    Debug.Log("Created Room");
    Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
#endif
    }

    public override void OnJoinedRoom()
    {
#if On
    Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
    Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
#endif
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player. Value.ActorNumber}");
        }

        PhotonNetwork.Instantiate("Player", SpawnLocation.transform.position, SpawnLocation.transform.rotation, 0);
        
        // 손 위치는 플레이어 기준으로 살짝 옆에 생성
        Vector3 leftOffset = SpawnLocation.transform.position + Vector3.left * 0.2f;
        Vector3 rightOffset = SpawnLocation.transform.position + Vector3.right * 0.2f;

        PhotonNetwork.Instantiate("LeftHand", leftOffset, Quaternion.identity, 0);
        PhotonNetwork.Instantiate("RightHand", rightOffset, Quaternion.identity, 0);
    }

}
