using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float serveUpwardForce = 3f;
    public float hittingForce = 1f;

    [SerializeField] private GameObject racket;
    [SerializeField] private GameObject mockRacket;
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

        MoveRacket();
    }

    private void HandleServe()
    {
        ball.transform.position = mockBall.transform.position;

        if (serving.IsPressed())
        {
            isServing = false;

            ball.transform.rotation = Quaternion.identity;

            var ballRb = ball.GetComponent<Rigidbody>();
            ballRb.AddForce(Vector3.up * serveUpwardForce, ForceMode.Impulse);
        }
    }

    private void MoveRacket()
    {
        var racketRb = racket.GetComponent<Rigidbody>();

        //racketRb.transform.position = mockRacket.transform.position;
        //racketRb.transform.rotation = mockRacket.transform.rotation;
        racketRb.transform.SetPositionAndRotation(mockRacket.transform.position, mockRacket.transform.rotation);
    }

    public void LeaveRoom()
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.LeaveRoom();
    }
}