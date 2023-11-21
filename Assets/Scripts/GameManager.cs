using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float serveUpwardForce = 3f;

    [SerializeField] private Racket player1;
    [SerializeField] private Racket player2;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject mockBall;

    private bool isServing;
    private PlayerInputActions playerActions;
    private InputAction serving;
    private InputAction restart;

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
        Reset();

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
    }

    private void Reset()
    {
        isServing = true;
    }

    private void Update()
    {
        if (isServing)
        {
            HandleServe();
        }

        if (serving.IsPressed() && restart.IsPressed())
        {
            Reset();
        }

        //var currentPlayer = GetCurrentPlayer();
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
}