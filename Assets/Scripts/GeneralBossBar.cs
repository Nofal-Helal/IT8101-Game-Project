using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralBossBar : MonoBehaviour
{
  public string bossName;
  TextMeshProUGUI bossNameUI;
  Image fillImage;
  public float Fill
  {
    get { return fillImage.fillAmount; }
    set { fillImage.fillAmount = value; }
  }
  void Start()
  {
    if (!transform.GetChild(1).TryGetComponent(out fillImage))
    {
      Debug.LogError("Could not find health bar fill image component");
      return;
    }
    if (!transform.GetChild(2).TryGetComponent(out bossName))
    {
      Debug.LogError("Could not find the name section");
      return;
    }
    bossNameUI.text = bossName;
  }
}
