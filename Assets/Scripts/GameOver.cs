using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    void Start()
    {
        scoreText.text = "Score: " + Global.currentLevelScore.ToString();
    }
    public void OnPressRetry()
    {
        SceneTransition.Fade("Scenes/Main Menu");
    }

    public void OnRetryPressMainMenu()
    {
        SceneTransition.Fade("Scenes/Main Menu");
    }
}
