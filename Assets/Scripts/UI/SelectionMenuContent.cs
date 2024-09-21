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
        if (saveData is PlayerData)
        {
            PlayerData playerSave = (PlayerData)saveData;
            GameManager.Instance.SetPlayer(playerSave);
            MainMenu.Instance.ResetPlayerButtons();
        }
        else if (saveData is WorldData)
        {
            WorldData worldSave = (WorldData)saveData;
            GameManager.Instance.SetWorld(worldSave);
            MainMenu.Instance.ResetWorldButtons();
        }

        image.color = selectedColor;
    }

    public void OnDelete()
    {
        if (saveData is PlayerData)
        {
            PlayerData playerSave = (PlayerData)saveData;
            SaveSystem.DeleteSave(playerSave);
        }
        else if (saveData is WorldData)
        {
            WorldData worldSave = (WorldData)saveData;
            SaveSystem.DeleteSave(worldSave);
        }

        Destroy(gameObject);
    }

    public void ResetButton()
    {
        image.color = Color.white;
    }
}
