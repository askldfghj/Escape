using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtnScript : MonoBehaviour
{
    void StartGame()
    {
        SceneManager.LoadScene("1_play");
    }

}
