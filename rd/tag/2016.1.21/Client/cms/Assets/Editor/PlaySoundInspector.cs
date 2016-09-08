using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlaySound))]
public class PlaySoundInspector : Editor
{
    PlaySound audio;
    public override void OnInspectorGUI()
    {
        audio = target as PlaySound;
        SerializedProperty prop = serializedObject.FindProperty("type");
        if (prop != null)
        {
            EditorGUILayout.PropertyField(prop, new GUIContent("Sound Type"), GUILayout.MinWidth(40f));
            audio.Type = (SoundType)prop.intValue;
        }
        SerializedProperty sound1 = serializedObject.FindProperty("sound1");
        SerializedProperty sound2 = serializedObject.FindProperty("sound2");
        if (sound1 == null || sound2 == null) return;
        if (audio.Type == SoundType.UI)
        {
            EditorGUILayout.PropertyField(sound1, new GUIContent("Open Sound"), GUILayout.MinWidth(40f));
            audio.Sound1 = sound1.stringValue;
            EditorGUILayout.PropertyField(sound2, new GUIContent("Close Sound"), GUILayout.MinWidth(40f));
            audio.Sound2 = sound2.stringValue;
        }
        else if (audio.Type == SoundType.Tips)
        {
            EditorGUILayout.PropertyField(sound1, new GUIContent("Tips Sound"), GUILayout.MinWidth(40f));
            audio.Sound1 = sound1.stringValue;
        }
        else if (audio.Type == SoundType.Click)
        {
            EditorGUILayout.PropertyField(sound1, new GUIContent("Click Sound"), GUILayout.MinWidth(40f));
            audio.Sound1 = sound1.stringValue;
        }
    }
}
