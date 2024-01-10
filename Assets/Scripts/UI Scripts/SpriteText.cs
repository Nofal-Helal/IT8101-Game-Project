using TMPro;
using UnityEngine.InputSystem;

public static class SpriteText
{
    public static void SetText(this TextMeshProUGUI tmp, string formatString, InputBinding binding, TMP_SpriteAsset spriteAsset)
    {
        string spriteName = RenameInput(binding.ToString());
        string spriteText = $"<sprite=\"{spriteAsset.name}\" name=\"{spriteName}\">";
        tmp.text = string.Format(formatString, spriteText);
    }


    public static string RenameInput(string buttonName)
    {
        int i = buttonName.LastIndexOf(':') + 1;
        buttonName = buttonName[i..];
        buttonName = buttonName.Replace("<Keyboard>/", "Key_");
        buttonName = buttonName.Replace("<Mouse>/", "Mouse_");
        return buttonName;
    }
}
