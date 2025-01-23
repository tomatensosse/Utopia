using Sirenix.OdinInspector;

[System.Serializable]
public struct CSParameter
{
    [HorizontalGroup("Header")]
    [HideLabel]
    public string uid;

    [HorizontalGroup("Header")]
    [HideLabel]
    public string displayName;

    [MultiLineProperty(3)]
    public string description;

    [HorizontalGroup("Type", Width = 0.5f)]
    public CSParameterType type;

    [HorizontalGroup("Type")]
    [ShowIf("@type != CSParameterType.Buffer")]
    public object defaultValue;

    public CSParameter(string uid, string displayName, string description, CSParameterType type, object defaultValue)
    {
        this.uid = uid;
        this.displayName = displayName;
        this.description = description;
        this.type = type;
        this.defaultValue = defaultValue;
    }
}

public enum CSParameterType
{
    Int,
    Float,
    Vector2,
    Vector3,
    Vector4,
    Bool,
    Buffer
}