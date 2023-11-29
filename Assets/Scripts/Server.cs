using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Server : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 4;
    public Dictionary<string, RoomInfo> cachedRoomList = new();

    [SerializeField]
    private GameObject controlPanel;
    [SerializeField]
    private GameObject progressLabel;
    [SerializeField]
    private TextMeshProUGUI lobbyCode;

    [SerializeField] private LobbyManager lobbyManager;

    private readonly string gameVersion = "1";

    public string tag;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null) { PhotonNetwork.LeaveRoom(); }
        if (PhotonNetwork.CurrentLobby != null) { PhotonNetwork.LeaveLobby(); }

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.JoinLobby();

        PhotonNetwork.LocalPlayer.TagObject = tag;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");

        PhotonNetwork.JoinLobby(TypedLobby.Default);

        lobbyManager.OnConnected();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");

        cachedRoomList.Clear();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed joining random room: {message}");
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");

        cachedRoomList.Clear();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (var i = 0; i < roomList.Count; i++)
        {
            var info = roomList[i];

            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }

        Debug.Log($"Room count: {cachedRoomList.Count}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer);
        Debug.Log(newPlayer.NickName);
        lobbyManager.OnPlayerJoined(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        lobbyManager.OnPlayerLeft(otherPlayer.IsMasterClient);
    }

    public void Play()
    {
        Debug.Log("Loading game");
        PhotonNetwork.LoadLevel("Game");
    }

    public void CreateRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            Debug.Log("Already on a room");
            return;
        }

        Debug.Log("Creating room...");

        var lobbyId = Random.Range(1000, 9999);
        var roomOptions = new RoomOptions()
        {
            MaxPlayers = maxPlayers,
        };
        PhotonNetwork.CreateRoom(lobbyId.ToString(), roomOptions, typedLobby: TypedLobby.Default);

        SetLobbyCode(lobbyId.ToString());
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void JoinRoom(string roomId)
    {        
        Debug.Log($"Joining room {roomId}");
        
        PhotonNetwork.JoinRoom(roomId);

        SetLobbyCode(roomId);
    }

    private void SetLobbyCode(string lobbyId)
    {
        lobbyCode.text = $"Lobby code: {lobbyId}";
    }

    public List<Player> GetPlayersInCurrentRoom()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            return new List<Player>();
        }

        return PhotonNetwork.CurrentRoom.Players.Select(p => p.Value).ToList();
    }
}
