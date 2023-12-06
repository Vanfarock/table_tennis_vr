using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float serveUpwardForce = 3f;
    public int maxPoints = 12;

    [SerializeField] private Racket player1;
    [SerializeField] private Racket player2;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject mockBall;
    [SerializeField] private AreaNet net;
    [SerializeField] private AreaP1 areaP1;
    [SerializeField] private AreaP2 areaP2;

    private bool isServing;
    private PlayerInputActions playerActions;
    private InputAction serving;
    private InputAction restart;
    private Racket refPlayer;
    private PhotonView photonView;

    private void Awake()
    {
        playerActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        serving = playerActions.Actions.Serving;
        restart = playerActions.Actions.Reset;
        playerActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }

    private void Start()
    {
        ResetGame();

        if (PhotonNetwork.IsMasterClient)
        {
            player1.SetCurrentPlayer();
            player2.UnsetCurrentPlayer();
        }
        else
        {
            player1.UnsetCurrentPlayer();
            player2.SetCurrentPlayer();
        }

        photonView = GetComponent<PhotonView>();
    }

    private void ResetGame()
    {
        ResetServe();
        player1.points = 0;
        player2.points = 0;
    }

    private void ResetServe()
    {
        isServing = true;
        SetRefPlayer(player1);
    }

    public void SetRefPlayer(Racket player)
    {
        refPlayer = player;
        ResetCont();
    }

    private void ResetCont()
    {
        net.cont = 0;
        areaP1.cont = 0;
        areaP2.cont = 0;
    }

    private void Update()
    {
        if (isServing)
        {
            HandleServe();
        }

        if (serving.IsPressed() && restart.IsPressed())
        {
            ResetGame();
        }

        
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncPositions", RpcTarget.OthersBuffered);
        }
    }

    [PunRPC]
    private void SyncPositions(Racket player1, GameObject ball)
    {
        this.ball.transform.position = ball.transform.position;
        this.player1.transform.SetPositionAndRotation(player1.transform.position, player1.transform.rotation);
    }

    private void HandleServe()
    {
        var ballRb = ball.GetComponent<Rigidbody>();
        ball.transform.position = mockBall.transform.position;

        if (serving.IsPressed())
        {
            isServing = false;

            ball.transform.rotation = Quaternion.identity;
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;

            ballRb.AddForce(Vector3.up * serveUpwardForce, ForceMode.Impulse);
        }
    }

    private Racket GetCurrentPlayer()
    {
        if (player1.currentPlayer)
        {
            return player1;
        }
        return player2;
    }

    public void LeaveRoom()
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.LeaveRoom();
    }

    private Racket GetOtherPlayer() 
    { 
        if (refPlayer == player1)
        {
            return player2;
        }
        else
        {
            return player1;
        }
    }

    internal void AddPointRef()
    {
        refPlayer.points += 1;
        Debug.Log("P1 points: "+player1.points);
        Debug.Log("P2 points: "+player2.points);
        if (refPlayer.points == maxPoints)
        {
            ResetGame();
        } 
        else
        {
            ResetServe();
        }
    }

    internal void AddPointOther()
    {
        Racket otherPlayer = GetOtherPlayer();
        otherPlayer.points += 1;
        Debug.Log("P1 points: "+player1.points);
        Debug.Log("P2 points: "+player2.points);
        if (otherPlayer.points == maxPoints)
        {
            ResetGame();
        }
        else
        {
            ResetServe();
        }

    }
}