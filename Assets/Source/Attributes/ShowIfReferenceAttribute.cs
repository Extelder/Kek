using UnityEngine;

public class ShowIfReferenceAttribute : PropertyAttribute
{
    public string ConditionField;

    public ShowIfReferenceAttribute(string conditionField)
    {
        ConditionField = conditionField;
    }
}

