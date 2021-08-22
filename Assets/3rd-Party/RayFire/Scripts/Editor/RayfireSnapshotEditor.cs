using UnityEngine;
using UnityEditor;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireSnapshot))]
    public class RayfireSnapshotEditor : Editor
    {
        // Target
        RayfireSnapshot snap;

        public override void OnInspectorGUI()
        {
            // Get target
            snap = target as RayfireSnapshot;
            if (snap == null)
                return;
            
            GUILayout.Space (8);

            // Save
            if (snap.transform.childCount > 0)
                if (GUILayout.Button ("Snapshot", GUILayout.Height (25)))
                    snap.Snapshot();

            // Load
            if (snap.snapshotAsset != null)
                if (GUILayout.Button ("Load", GUILayout.Height (25)))
                    snap.Load();

            // Draw script UI
            DrawDefaultInspector();
        }
    }
}