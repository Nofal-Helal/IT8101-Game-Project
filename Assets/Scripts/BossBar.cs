using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
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
        }
    }

}
