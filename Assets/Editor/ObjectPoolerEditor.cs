using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPooler))]        // state which script this is the custome editor for

public class ObjectPoolerEditor : Editor
{
    /// <summary>
    /// The serialized property that links to the PooledObject array from the object pooler
    /// </summary>
    SerializedProperty m_PooledObject;

    /// <summary>
    /// The serialized property that links to the PooledAmount array from the object pooler
    /// </summary>
    SerializedProperty m_PooledAmount;

    /// <summary>
    /// The serialized property that links to the WillGrow array from the object pooler
    /// </summary>
    SerializedProperty m_WillGrow;

    void OnEnable()
    {
        m_PooledObject = serializedObject.FindProperty("m_PooledObject");       // set the Serialized Properties (serializedObject is the Object the script is attached to
        m_PooledAmount = serializedObject.FindProperty("m_PooledAmount");
        m_WillGrow = serializedObject.FindProperty("m_WillGrow");
    }

    /// <summary>
    /// We override this method as it's called every time the inspector is updated
    /// </summary>
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_PooledObject.FindPropertyRelative("Array.size"));   // create a property field that allows us to set the size of the m_PooledObject array

        m_PooledAmount.arraySize = m_PooledObject.arraySize;        // set the size of the m_PooledAmount and m_WillGrow arrays based on the size of the m_PooledObject array
        m_WillGrow.arraySize = m_PooledObject.arraySize;

        GUILayout.BeginHorizontal();                                // begin horizontal will put the following label fields in a horizontal row
        EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth;  // set the label width based on the field width
        EditorGUILayout.LabelField("Object");                       // adds a label that prints out the following strings
        EditorGUILayout.LabelField("Amount");
        EditorGUILayout.LabelField("Will Grow");
        GUILayout.EndHorizontal();                                  // ends the horizontal layout group

        for (int i = 0; i < m_PooledObject.arraySize; i++)      // loop through the m_PooledObject array size
        {
            GUILayout.BeginHorizontal();                                                                // start another horizontal layout group
            EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth;                                  // set the width
            EditorGUILayout.PropertyField(m_PooledObject.GetArrayElementAtIndex(i), GUIContent.none);   // create property fields that are the value of the specified array index
            EditorGUILayout.PropertyField(m_PooledAmount.GetArrayElementAtIndex(i), GUIContent.none);
            EditorGUILayout.PropertyField(m_WillGrow.GetArrayElementAtIndex(i), GUIContent.none);
            GUILayout.EndHorizontal();                                                                  // end the horizontal layout
            GUILayout.Space(10);                                                                        // add a space
        }

        serializedObject.ApplyModifiedProperties();     // apply the changes made to the serialized object
        serializedObject.Update();                      // update the values
    }
}
