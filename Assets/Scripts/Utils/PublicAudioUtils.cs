#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Practear.Utils
{
    /// <summary>
    /// Class used to play an audio clip from the editor using reflection
    /// </summary>
    static public class PublicAudioUtils
    {

        /// <summary>
        /// Play the clip
        /// </summary>
        /// <param name="clip">The audio clip.</param>
        public static void PlayClip(AudioClip clip)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] {
                typeof(AudioClip)
                },
                null
            );
            method.Invoke(
                null,
                new object[] {
                clip
                }
            );
        }

        /// <summary>
        /// Stop all the audio clips
        /// </summary>
        public static void StopAllClips()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass =
                  unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] { },
                null
            );
            method.Invoke(
                null,
                new object[] { }
            );
        }
    }
}

#endif
