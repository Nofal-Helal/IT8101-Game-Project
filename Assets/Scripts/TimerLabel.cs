using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerLabel : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private int time;
    private void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(UpdateText), 0, 1);
    }

    private void UpdateText()
    {
        int mins = time / 60;
        int secs = time % 60;
        tmp.text = $"Time:  {mins}:{secs}";
        time += 1;
    }
}
