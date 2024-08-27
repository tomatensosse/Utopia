using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMenuContent : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text displayText;
    public Image image;
    public Color selectedColor;
    [Header("Save Data")]
    public SaveData saveData;

    public void OnPress()
    {
        if (saveData is PlayerSave)
        {
            PlayerSave playerSave = (PlayerSave)saveData;
            GameManager.Instance.SetPlayer(playerSave);
            MainMenu.Instance.ResetPlayerButtons();
        }
        else if (saveData is WorldSave)
        {
            WorldSave worldSave = (WorldSave)saveData;
            GameManager.Instance.SetWorld(worldSave);
            MainMenu.Instance.ResetWorldButtons();
        }

        image.color = selectedColor;
    }

    public void ResetButton()
    {
        image.color = Color.white;
    }
}
