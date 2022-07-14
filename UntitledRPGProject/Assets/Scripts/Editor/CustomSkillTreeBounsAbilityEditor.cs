using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(SkillTree_BounsAbility))]
public class CustomSkillTreeBounsAbilityEditor : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var effectRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var secondRect = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);

        var type = property.FindPropertyRelative("Type");
        var value = property.FindPropertyRelative("Value");
        type.intValue = EditorGUI.Popup(effectRect, "Type", type.intValue, type.enumNames);

        switch ((SkillTree_BounsAbility.SkillTreeAbilityType)type.intValue)
        {
            case SkillTree_BounsAbility.SkillTreeAbilityType.Movement:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.Damage:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.Mana:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.Health:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.Armor:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.DoubleAttack:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.Shield:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.HPRegeneration:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            case SkillTree_BounsAbility.SkillTreeAbilityType.MPRegeneration:
                value.floatValue = EditorGUI.FloatField(secondRect, "Percentage", value.floatValue);
                break;
            default:
                break;
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    //This will need to be adjusted based on what you are displaying
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 2);
    }
}
