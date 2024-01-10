using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    void Start()
    {
        scoreText.text = "Score: " + Global.playerData.score.ToString();

        // lose score and gold gained during the failed attempt
        Global.playerData.score = Global.lastScore;
        Global.playerData.gold = Global.lastGold;
    }
    public void OnPressRetry()
    {
        MenuNavigation.LoadGame();
    }

    public void OnRetryPressMainMenu()
    {
        SceneTransition.Fade("Scenes/Main Menu");
    }
}
