using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string playScene;

    public void StartGame()
    {
        SceneManager.LoadScene(playScene);
    }

    public void Quit()
    {
       Application.Quit();
    }
}
