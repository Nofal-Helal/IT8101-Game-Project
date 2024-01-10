using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    [NaughtyAttributes.Button]
    public void DebugBindings()
    {
        foreach (var binding in Global.inputActions.bindings
        )
        {
            Debug.Log(SpriteText.RenameInput(binding.ToString()));
        }

    }

    public TMP_SpriteAsset spriteAsset;
    public TextMeshProUGUI shootButton;
    public TextMeshProUGUI reloadButton;
    public TextMeshProUGUI removeObstaclesButton;

    public void Start()
    {
        if (shootButton != null)
            shootButton.SetText("Press {0} to shoot", Global.inputActions.gameplay.Shoot.bindings[0], spriteAsset);
        if (reloadButton != null)
            reloadButton.SetText("Press {0} to reload", Global.inputActions.gameplay.Reload.bindings[0], spriteAsset);
        if (removeObstaclesButton != null)
            removeObstaclesButton.SetText("Press and hold {0} to remove obstacles", Global.inputActions.gameplay.RemoveObstacle.bindings[0], spriteAsset);
    }

}
