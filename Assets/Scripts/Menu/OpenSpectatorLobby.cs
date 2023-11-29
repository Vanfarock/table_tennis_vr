using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class OpenSpectatorLobby : MonoBehaviour
    {
        public void LoadSpectatorLobby()
        {
            Debug.Log("Loading spectator lobby");
            SceneManager.LoadScene("SpectatorLobby");
        }
    }
}
