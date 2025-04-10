using _01_Scripts.GameState;
using _01_Scripts.GameState.States;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string playScene;

    public void StartGame()
    {
        GameStateController.Instance.ChangeState(new ShipEditor_GameState());
        SceneManager.LoadScene(playScene);
    }

    public void Quit()
    {
       Application.Quit();
    }
}
