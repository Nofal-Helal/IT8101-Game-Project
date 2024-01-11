using System.Collections;
using TMPro;
using UnityEngine;

public class TransitionSceneScript : MonoBehaviour
{
    public static string nextScene;
    public Animator levelNumber;
    public TextMeshProUGUI scoreText;
    void Start()
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float duration = 3f;
        int score = Global.playerData.score;
        int scoreStart = Global.lastScore;
        for (float time = 0; time <= duration; time += Time.deltaTime)
        {
            int scoreAnim = scoreStart + Mathf.CeilToInt(time / duration * (score - scoreStart));
            scoreText.text = "Score: " + scoreAnim.ToString();
            yield return 0;
        }
        scoreText.text = "Score: " + score.ToString();
        yield return new WaitForSeconds(0.5f);
        levelNumber.enabled = true;
        yield return new WaitForSeconds(1 + levelNumber.GetCurrentAnimatorStateInfo(0).length);
        SceneTransition.Fade(nextScene);
    }
}
