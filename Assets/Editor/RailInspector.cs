using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public struct PropertyAction
{
    public SerializedProperty property;
    public Action action;
    public PropertyAction(SerializedProperty property, Action action)
    {
        this.property = property; this.action = action;
    }

    public static IEnumerable<PropertyAction> PropertyActions(SerializedObject serialisedObject, params (string, Action)[] pas)
    {
        return pas.Select(pa => new PropertyAction(serialisedObject.FindProperty(pa.Item1), pa.Item2));
    }
}

[CustomEditor(typeof(RailRenderer))]
public class RailInspector : Editor
{
    IEnumerable<PropertyAction> propertyActions;
    void OnEnable()
    {
        Action UpdateMesh = () => (target as RailRenderer).UpdateMesh();
        propertyActions = PropertyAction.PropertyActions(
            serializedObject,
            ("plankSpacing", UpdateMesh),
            ("railWidth", UpdateMesh),
            ("railDetail", UpdateMesh),
            ("railScale", UpdateMesh));
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        foreach (var propertyAction in propertyActions)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propertyAction.property);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                propertyAction.action();
            }
        }

        if (GUILayout.Button("Regenerate Mesh"))
        {
            (target as RailRenderer).UpdateMesh();

        }

    }
}
