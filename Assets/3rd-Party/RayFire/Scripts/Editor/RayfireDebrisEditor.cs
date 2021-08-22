using UnityEngine;
using UnityEditor;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireDebris))]
    public class RayfireDebrisEditor : Editor
    {
        // Target
        RayfireDebris debris = null;

        public override void OnInspectorGUI()
        {
            // Get target
            debris = target as RayfireDebris;
            if (debris == null)
                return;
            
            // Space
            GUILayout.Space (8);
            
            
            GUILayout.BeginHorizontal();
            
            if (Application.isPlaying == true)
            {
                if (GUILayout.Button ("Emit", GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireDebris != null)
                                (targ as RayfireDebris).Emit();
                
                if (GUILayout.Button ("Clean", GUILayout.Height (25)))
                    foreach (var targ in targets)
                        if (targ as RayfireDebris != null)
                            (targ as RayfireDebris).Clean();
            }

            EditorGUILayout.EndHorizontal();
            
            // Draw script UI
            DrawDefaultInspector();
            
            // Space
            GUILayout.Space (8);
        }
    }
}