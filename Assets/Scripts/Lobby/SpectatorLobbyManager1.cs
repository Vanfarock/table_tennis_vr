using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpectatorLobbyManager : MonoBehaviour
{
    private Color selectedBackgroundColor = new(142f/255, 139f/255, 139f/255);
    private Color selectedTextColor = new(1f, 1f, 1f);
    
    private Color unselectedBackgroundColor = new(207f/255, 207f/255, 207f/255);
    private Color unselectedTextColor = new(50f/255, 50f/255, 50f/255);

    private bool isJoinSelected = true;

    [SerializeField] private GameObject loadingText;

    [SerializeField] private GameObject joinButton;

    [SerializeField] private GameObject joinPanel;

    [SerializeField] private GameObject roomsScrollPanelContainer;
    [SerializeField] private Button scrollPanelItemPrefab;

    [SerializeField] private GameObject youContainer;
    [SerializeField] private TextMeshProUGUI youName;

    [SerializeField] private GameObject otherPlayerContainer;
    [SerializeField] private TextMeshProUGUI otherPlayerName;

    //[SerializeField] private GameObject startGameButton;

    [SerializeField] private Server server;

    private void Start()
    {
        loadingText.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnConnected()
    {
        loadingText.SetActive(false);
        gameObject.SetActive(true);

        LoadAvailableRooms();

        StartCoroutine(CheckNewRooms());
    }

    private IEnumerator CheckNewRooms()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            LoadAvailableRooms();
        }
    }

    public void LoadAvailableRooms()
    {
        foreach (Transform child in roomsScrollPanelContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in server.cachedRoomList)
        {
            var row = Instantiate(
                original: scrollPanelItemPrefab,
                parent: roomsScrollPanelContainer.transform
            );
            row.transform.localScale = Vector3.one;
            row.gameObject.SetActive(true);

            row.onClick.AddListener(() => OnJoinRoom(row));

            row.GetComponentInChildren<TextMeshProUGUI>().text = $"Room {item.Key}";
        }
    }

    private void OnJoinRoom(Button clickedButton)
    {
        var roomId = clickedButton.GetComponentInChildren<TextMeshProUGUI>().text.Replace("Room ", "");

        server.JoinRoom(roomId);

        joinPanel.SetActive(false);

        var players = server.GetPlayersInCurrentRoom();

        if (players.Count > 0)
        {
            youContainer.SetActive(true);
            youName.text = players[0].NickName;
        }
        else
        {
            youName.text = "";
        }

        if (players.Count > 1)
        {
            otherPlayerContainer.SetActive(true);
            otherPlayerName.text = players[1].NickName;
        }
        else
        {
            otherPlayerName.text = "";
        }

        //startGameButton.SetActive(false);

        isJoinSelected = false;
    }

    public void OnJoinSelect()
    {
        if (isJoinSelected) { return; }

        server.LeaveRoom();

        Select(joinButton);

        joinPanel.SetActive(true);

        //startGameButton.SetActive(false);

        LoadAvailableRooms();

        isJoinSelected = true;
    }

    public void OnCreateSelect()
    {
        if (!isJoinSelected) { return; }


        joinPanel.SetActive(false);

        youContainer.SetActive(true);
        youName.text = PhotonNetwork.NickName;
        otherPlayerContainer.SetActive(false);

        //startGameButton.SetActive(true);

        isJoinSelected = false;
    }

    private void Select(GameObject component)
    {
        component.GetComponent<Image>().color = selectedBackgroundColor;
        component.GetComponentInChildren<TextMeshProUGUI>().color = selectedTextColor;
    }

    private void Unselect(GameObject component)
    {
        component.GetComponent<Image>().color = unselectedBackgroundColor;
        component.GetComponentInChildren<TextMeshProUGUI>().color = unselectedTextColor;
    }

    public void OnPlayerJoined(string playerName)
    {
        otherPlayerContainer.SetActive(true);
        otherPlayerName.text = playerName;
    }

    public void OnPlayerLeft(bool isMasterClient)
    {
        otherPlayerContainer.SetActive(false);
        //startGameButton.SetActive(true);
    }

    public void StartGame()
    {
        server.Play();
    }
}
