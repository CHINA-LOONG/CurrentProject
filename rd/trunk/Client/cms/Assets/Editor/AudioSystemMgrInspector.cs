using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AudioSystemMgr))]
public class AudioSystemMgrInspector : Editor
{

    public override void OnInspectorGUI()
    {
        AudioSystemMgr audioSystem = target as AudioSystemMgr;

        audioSystem.MusicVolume = EditorGUILayout.Slider("Music Volume", audioSystem.MusicVolume, 0.0f, 1.0f);

        audioSystem.SoundVolume = EditorGUILayout.Slider("Sound Volume", audioSystem.SoundVolume, 0.0f, 1.0f);

    }

}
