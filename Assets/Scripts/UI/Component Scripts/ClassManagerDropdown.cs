using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using System;
using System.IO;

[System.Serializable]
public class CharacterProperties
{
    public int baseHP;
    public int baseStamina;
    public int baseArmor;
    public string[] startingItems;
}

[System.Serializable]
public class CharacterClass // Renamed from Character to CharacterClass
{
    public string name;
    public string description;
    public string icon; // Add this line
    public CharacterProperties properties; // Add this line
}

[System.Serializable]
public class CharacterList
{
    public CharacterClass[] classes; // Updated to use CharacterClass
}

public class ClassManagerDropdown : MonoBehaviour
{
    public TMP_Dropdown characterDropdown; // Assign in the Inspector
    public TextAsset jsonFile; // Assign your JSON file in the Inspector

    void Start()
    {
        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        // Read the JSON data from the TextAsset
        string jsonData = jsonFile.text;

        // Deserialize JSON into the CharacterList class
        CharacterList characterList = JsonUtility.FromJson<CharacterList>(jsonData);
        
        // Prepare a list to hold the dropdown options
        List<string> dropdownOptions = new List<string>();
        dropdownOptions.Add("Choose Class");

        // Iterate through the characters
        foreach (CharacterClass character in characterList.classes) // Updated to CharacterClass
        {
            dropdownOptions.Add($"{character.name}: {character.description}");
            Debug.Log($"{character.name}: {character.description}");
        }

        // Populate the dropdown
        characterDropdown.ClearOptions();
        characterDropdown.AddOptions(dropdownOptions);
    }
}