using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Family", menuName = "NPCs/Family")]
public class Family : ScriptableObject
{
    public string familyID;
    public string familyName;
    public float baseTolerance = 0.5f;
    public List<FamilyRelation> relations;
    public Dictionary<string, float> relationsDict = new Dictionary<string, float>();

#if UNITY_EDITOR
    public List<FamilyRelation> relationsDictionaryToListEditor;

    [ContextMenu("Display Dictionary")]
    public void DisplayDictionary()
    {
        relationsDictionaryToListEditor = new List<FamilyRelation>();

        foreach (KeyValuePair<string, float> relation in relationsDict)
        {
            FamilyRelation newRelation = new FamilyRelation();
            newRelation.relationFamilyID = relation.Key;
            newRelation.relationValue = relation.Value;

            relationsDictionaryToListEditor.Add(newRelation);
        }
    }
#endif

    public void Initialize()
    {
        foreach (FamilyRelation relation in relations)
        {
            relationsDict.Add(relation.relationFamilyID, relation.relationValue);
        }
    }

    public void SetRelation(string familyID, float value)
    {
        if (!relationsDict.ContainsKey(familyID))
        {
            relationsDict.Add(familyID, value);
            return;
        }

        relationsDict[familyID] = value;
    }

    public void AddRelation(string familyID, float value)
    {
        if (!relationsDict.ContainsKey(familyID))
        {
            relationsDict.Add(familyID, value);
            return;
        }

        relationsDict[familyID] += value;
    }
}
