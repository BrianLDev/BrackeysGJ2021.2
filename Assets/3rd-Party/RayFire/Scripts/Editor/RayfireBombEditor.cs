using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireBomb))]
    public class RayfireBombEditor : Editor
    {
        RayfireBomb  bomb;
        List<string> layerNames;
        
        // Draw gizmo
        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireBomb bomb, GizmoType gizmoType)
        {
            if (bomb.showGizmo == true)
            {
                // Vars
                float       rate          = 0f;
                const int   size          = 45;
                const float scale         = 1f / size;
                Vector3     previousPoint = Vector3.zero;
                Vector3     nextPoint     = Vector3.zero;
                Color       wireColor     = new Color (0.58f, 0.77f, 1f);

                // Gizmo properties
                Gizmos.color  = wireColor;
                Gizmos.matrix = bomb.transform.localToWorldMatrix;

                // Draw top eye
                rate            = 0f;
                nextPoint.y     = 0f;
                previousPoint.y = 0f;
                previousPoint.x = bomb.range * Mathf.Cos (rate);
                previousPoint.z = bomb.range * Mathf.Sin (rate);
                for (int i = 0; i < size; i++)
                {
                    rate        += 2.0f * Mathf.PI * scale;
                    nextPoint.x =  bomb.range * Mathf.Cos (rate);
                    nextPoint.z =  bomb.range * Mathf.Sin (rate);
                    Gizmos.DrawLine (previousPoint, nextPoint);
                    previousPoint = nextPoint;
                }

                // Draw top eye
                rate            = 0f;
                nextPoint.x     = 0f;
                previousPoint.x = 0f;
                previousPoint.y = bomb.range * Mathf.Cos (rate);
                previousPoint.z = bomb.range * Mathf.Sin (rate);
                for (int i = 0; i < size; i++)
                {
                    rate        += 2.0f * Mathf.PI * scale;
                    nextPoint.y =  bomb.range * Mathf.Cos (rate);
                    nextPoint.z =  bomb.range * Mathf.Sin (rate);
                    Gizmos.DrawLine (previousPoint, nextPoint);
                    previousPoint = nextPoint;
                }

                // Draw top eye
                rate            = 0f;
                nextPoint.z     = 0f;
                previousPoint.z = 0f;
                previousPoint.y = bomb.range * Mathf.Cos (rate);
                previousPoint.x = bomb.range * Mathf.Sin (rate);
                for (int i = 0; i < size; i++)
                {
                    rate        += 2.0f * Mathf.PI * scale;
                    nextPoint.y =  bomb.range * Mathf.Cos (rate);
                    nextPoint.x =  bomb.range * Mathf.Sin (rate);
                    Gizmos.DrawLine (previousPoint, nextPoint);
                    previousPoint = nextPoint;
                }

                // Selectable sphere
                float sphereSize = bomb.range * 0.07f;
                if (sphereSize < 0.1f)
                    sphereSize = 0.1f;
                Gizmos.color = new Color (1.0f, 0.60f, 0f);
                Gizmos.DrawSphere (new Vector3 (0f,          bomb.range,  0f),          sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,          -bomb.range, 0f),          sphereSize);
                Gizmos.DrawSphere (new Vector3 (bomb.range,  0f,          0f),          sphereSize);
                Gizmos.DrawSphere (new Vector3 (-bomb.range, 0f,          0f),          sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,          0f,          bomb.range),  sphereSize);
                Gizmos.DrawSphere (new Vector3 (0f,          0f,          -bomb.range), sphereSize);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere (new Vector3 (0f, 0f, 0f), sphereSize / 3f);

            }
        }

        private void OnSceneGUI()
        {
            var bomb      = target as RayfireBomb;
            var transform = bomb.transform;

            // Draw handles
            EditorGUI.BeginChangeCheck();
            bomb.range = Handles.RadiusHandle (transform.rotation, transform.position, bomb.range);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObject (bomb, "Change Range");
            }
        }

        // Inspector editing
        public override void OnInspectorGUI()
        {
            // Get target
            bomb = target as RayfireBomb;
            if (bomb == null)
                return;
            
            // Space
            GUILayout.Space (8);

            // Cache UI Begin
            GUILayout.BeginHorizontal();

            // Explode
            if (GUILayout.Button ("Explode", GUILayout.Height (25)))
            {
                bomb.Explode (bomb.delay);
                SetDirty (bomb);
            }

            // Restore
            if (GUILayout.Button ("Restore", GUILayout.Height (25)))
            {
                bomb.Restore();
                SetDirty (bomb);
            }
            
            // Cache UI End
            EditorGUILayout.EndHorizontal();

            // Space
            GUILayout.Space (1);

            // Fragmentation section Begin
            GUILayout.BeginHorizontal();

            // Show gizmo
            EditorGUI.BeginChangeCheck();
            bomb.showGizmo = GUILayout.Toggle (bomb.showGizmo, "Show Gizmo", "Button");
            if (EditorGUI.EndChangeCheck())
            {
                SetDirty (bomb);
                SceneView.RepaintAll();
            }
            
            // Fragmentation section End
            EditorGUILayout.EndHorizontal();

            // Space
            GUILayout.Space (3);

            // Draw script UI
            DrawDefaultInspector();

            // Space
            GUILayout.Space (5);

            // Label
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            
            // Tag filter
            bomb.tagFilter = EditorGUILayout.TagField ("Tag", bomb.tagFilter);

            // Layer mask
            if (layerNames == null)
                layerNames = new List<string>();
            for (int i = 0; i <= 31; i++)
                layerNames.Add (i + ". " + LayerMask.LayerToName (i));
            bomb.mask = EditorGUILayout.MaskField ("Layer", bomb.mask, layerNames.ToArray());
            
            if (EditorGUI.EndChangeCheck())
            {
                SetDirty (bomb);
                SceneView.RepaintAll();
            }
        }
        
        // Set dirty
        void SetDirty (RayfireBomb scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
            }
        }
    }
}