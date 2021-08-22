﻿using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

// Instantiate preserved

namespace RayFire
{
    [Serializable]
    public class RFReset
    {
        // Post dml object 
        public enum PostDemolitionType
        {
            DestroyWithDelay  = 0,
            DeactivateToReset = 1
        }
        
        // Mesh reuse
        public enum MeshResetType
        {
            Destroy              = 0,
            ReuseInputMesh       = 2,
            ReuseFragmentMeshes  = 4
        }
        
        // Fragments reuse
        public enum FragmentsResetType
        {
            Destroy     = 0,
            Reuse       = 2,
            Preserve    = 4
        }
        
        [Header ("  Reset")]

        [Tooltip ("Reset transform to position and rotation when object was initialized.")]
        public bool transform;
        [Space (2)]
        
        [Tooltip ("Reset damage value.")]
        public bool damage;
        [Space (3)]
        
        [Tooltip ("Reset Connectivity.")]
        public bool connectivity;

        [Header ("  Post Demolition")]
        [Space (3)]
        
        public PostDemolitionType action;
        [Space (2)]
        
        [Tooltip ("Object will be destroyed after defined delay.")]
        [Range (0, 60)] public float destroyDelay;

        [Header ("  Reuse")]
        [Space (3)]
        
        public MeshResetType mesh;
        [Space (2)]
        
        public FragmentsResetType fragments;
        [Space (2)]

        
        [NonSerialized] public bool toBeDestroyed;

        /// /////////////////////////////////////////////////////////
        /// Constructor
        /// /////////////////////////////////////////////////////////

        // Constructor
        public RFReset()
        {
            action        = PostDemolitionType.DestroyWithDelay;
            destroyDelay  = 1;
            transform     = true;
            damage        = true;
            mesh          = MeshResetType.ReuseFragmentMeshes;
            fragments     = FragmentsResetType.Destroy;
            toBeDestroyed = false;
        }

        // Copy from
        public void CopyFrom (RFReset reset, ObjectType objectTypeTo)
        {
            transform    = reset.transform;
            damage       = reset.damage;
            action       = reset.action;
            destroyDelay = reset.destroyDelay;
            
            // Copy to initial object: mesh root copy
            if (objectTypeTo == ObjectType.MeshRoot)
            {
                mesh      = reset.mesh;
                fragments = reset.fragments;
            }

            // Copy to cluster shards
            else if (objectTypeTo == ObjectType.ConnectedCluster)
            {
                mesh      = reset.mesh;
                fragments = reset.fragments;
            }
            
            // Copy to fragments
            else if (objectTypeTo == ObjectType.Mesh)
            {
                mesh      = MeshResetType.Destroy;
                fragments = FragmentsResetType.Destroy;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Rigid Mesh
        /// /////////////////////////////////////////////////////////
        
        // Rigid 
        public static void ResetRigid (RayfireRigid scr)
        {
            // Object can't be reused
            if (ObjectReuseState (scr) == false)
                return;
            
            // Mesh Root reset
            if (MeshRootReset (scr) == true)
                return;
            
            // Save faded/demolished state before reset
            int faded = scr.fading.state;
            bool demolished = scr.limitations.demolished;

            // Reset tm
            if (scr.reset.transform == true)
                RestoreTransform(scr);
            
            // Reset activation TODO check if it was Kinematic
            if (scr.activation.activated == true)
                scr.simulationType = SimType.Inactive;
            
            // Reset rigid props
            Reset (scr);
            
            // Stop all cors in case object restarted
            scr.StopAllCoroutines();
            
            // Reset if object fading/faded
            if (faded >= 1)
                ResetFade(scr);
            
            // Demolished. Restore
            if (demolished == true)
                ResetMeshDemolition (scr);
            
            // Restore cluster even if it was not demolished
            ResetClusterDemolition (scr);
            
            // Remove particles
            DestroyParticles (scr);
            
            // Enable Rigid because of cluster fade and reset
            if (scr.enabled == false)
                scr.enabled = true;
            
            // Activate if deactivated
            if (scr.gameObject.activeSelf == false)
                scr.gameObject.SetActive (true);

            // Start all coroutines
            scr.StartAllCoroutines();
        }

        // Reset if object fading/faded
        public static void ResetFade (RayfireRigid scr)
        {
            // Was excluded
            if (scr.fading.fadeType == FadeType.SimExclude)
            {
                // Null check because of Planar check fragments without collider
                if (scr.physics.meshCollider != null)
                    scr.physics.meshCollider.enabled = true;// TODO CHECK CLUSTER COLLIDERS
            }   
               
            // Was fall down
            else if (scr.fading.fadeType == FadeType.FallDown)
            {
                // Null check because of Planar check fragments without collider
                if (scr.physics.meshCollider != null)
                    scr.physics.meshCollider.enabled = true;// TODO CHECK CLUSTER COLLIDERS
                
                scr.gameObject.SetActive (true);
            } 
            
            // Was scaled down
            else if (scr.fading.fadeType == FadeType.ScaleDown)
            {
                scr.transForm.localScale = scr.physics.initScale;
                scr.gameObject.SetActive (true);
            }
            
            // Was moved down
            if (scr.fading.fadeType == FadeType.MoveDown)
            {
                // Null check because of Planar check fragments without collider
                if (scr.physics.meshCollider != null)
                    scr.physics.meshCollider.enabled = true; // TODO CHECK CLUSTER COLLIDERS

                // Reset gravity
                if (scr.simulationType != SimType.Inactive)
                    scr.physics.rigidBody.useGravity = scr.physics.useGravity;
                
                scr.gameObject.SetActive (true);
            }

            // Was destroyed
            else if (scr.fading.fadeType == FadeType.Destroy)
                scr.gameObject.SetActive (true);
            
            // Was set static
            if (scr.fading.fadeType == FadeType.SetStatic)
                scr.gameObject.SetActive (true);
            
            // Was set static
            if (scr.fading.fadeType == FadeType.SetKinematic)
                scr.gameObject.SetActive (true);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Rigid Mesh Root
        /// /////////////////////////////////////////////////////////

        // Mesh Root 
        static bool MeshRootReset (RayfireRigid scr)
        {
            // Not mesh root
            if (scr.objectType != ObjectType.MeshRoot)
                return false;

            // Cleanup destroyed/faded fragments
            if (MeshRootCleanup (scr) == false)
                return true;

            // Reset tm
            scr.transform.position = scr.physics.initPosition;
            scr.transform.rotation = scr.physics.initRotation;
            scr.transform.localScale = scr.physics.initScale;
            
            // Reset fragments first
            foreach (var fragment in scr.fragments)
            {
                // Add rigid body to Rigid if it was deleted because of clustering
                if (fragment.physics.rigidBody == null)
                {
                 
                    Rigidbody r = fragment.gameObject.GetComponent<Rigidbody>();
                    if (r != null)
                        Debug.Log (r.name);
                    
                    fragment.physics.rigidBody = fragment.gameObject.AddComponent<Rigidbody>();
                }

                // Set object type back in case of clustering->demolition
                fragment.simulationType = scr.simulationType;

                // Set parent in case of clustering->demolition
                fragment.transForm.parent = scr.transForm;
                
                // Reset rigid
                ResetRigid (fragment);
                
                // Set density. After collider defined TODO save mass at first apply, reuse now
                RFPhysic.SetDensity (fragment);

                // Set drag properties
                RFPhysic.SetDrag (fragment);
                
                // Destroy parent connected cluster if rigid was clustered
                if (fragment.rootParent != null)
                    Object.Destroy (fragment.rootParent.gameObject);
                
                // TODO Test fragments reuse with transform state copied to fragments

            }

            // Restore connectivity cluster
            RFBackupCluster.RestoreConnectivity (scr.activation.connect);
            
            return true;
        }

        // Cleanup and check for mesh root fragments
        static bool MeshRootCleanup (RayfireRigid scr)
        {
            // Cleanup destroyed/faded fragments
            for (int i = scr.fragments.Count - 1; i >= 0; i--)
                if (scr.fragments[i] == null)
                {
                    Debug.Log (scr.name + ": Mesh Root Fragment destroyed", scr.gameObject);
                    scr.fragments.RemoveAt (i);
                }

            // Check after cleanup
            if (scr.HasFragments == false)
                return false;

            return true;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Reset Rigid Root
        /// /////////////////////////////////////////////////////////
        
        // Reinit demolished mesh object
        public static void RigidRootReset (RayfireRigidRoot scr)
        {
            // Stop all cors in case object restarted
            scr.StopAllCoroutines();
            scr.corState                    = false;
            scr.activation.inactiveCorState = false;
            scr.fading.offsetCorState       = false;
            
            // Reset activation
            scr.activation.Reset();
            
            // TODO CHECK FOR RESET STATES
            // TODO CLEANUP
            
            // Destroy particle roots
            DestroyParticles (scr);
            
            // Reset tm
            scr.transform.position   = scr.cluster.pos;
            scr.transform.rotation   = scr.cluster.rot;
            scr.transform.localScale = scr.cluster.scl;
           
            // Set object type back in case of clustering->demolition
            for (int i = 0; i < scr.cluster.shards.Count; i++)
                if (scr.cluster.shards[i].rigid == null)
                    scr.cluster.shards[i].sm = scr.simulationType;
            
            // Reset uny states and sim state
            for (int i = 0; i < scr.unyList.Count; i++)
                scr.unyList[i].SetRigidRootUnyShardList();
            
            // Reset shards
            for (int i = 0; i < scr.cluster.shards.Count; i++)
            {
                // Set parent in case of clustering->demolition
                scr.cluster.shards[i].tm.parent     = scr.tm;
                
                // Set transform
                scr.cluster.shards[i].tm.position   = scr.cluster.shards[i].pos;
                scr.cluster.shards[i].tm.rotation   = scr.cluster.shards[i].rot;
                scr.cluster.shards[i].tm.localScale = scr.cluster.shards[i].scl;
                
                // Shard faded
                if (scr.cluster.shards[i].fade != 0)
                {
                    // Enable collider
                    if (scr.cluster.shards[i].col.enabled == false)
                        scr.cluster.shards[i].col.enabled = true;
                    
                    // Reset fading
                    scr.cluster.shards[i].fade = 0;
                }
                
                // TODO Destroy parent connected cluster if rigid was clustered
                
                // Activate
                if (scr.cluster.shards[i].tm.gameObject.activeSelf == false)
                    scr.cluster.shards[i].tm.gameObject.SetActive (true);
            }
            
            // Set physics properties for shards
            RFPhysic.SetPhysics(scr.cluster.shards, scr.physics);
            
            // Reset shards with Rigid
            for (int i = 0; i < scr.cluster.shards.Count; i++)
                if (scr.cluster.shards[i].rigid != null)
                    scr.cluster.shards[i].rigid.ResetRigid();
            
            // Setup list for activation shards
            scr.SetInactiveList ();

            // Setup list with fade by offset shards
            RFFade.SetOffsetFadeList (scr);

            // Destroy child clusters if they were created
            DestroyClusters (scr);

            // Restore connectivity cluster
            RFBackupCluster.RestoreConnectivity (scr.activation.connect);
            
            // Start coroutines
            scr.StartAllCoroutines();
        }

        // Destroy particles
        public static void DestroyParticles (RayfireRigidRoot scr)
        {
            if (scr.partList.Count > 0)
            {
                for (int i = scr.partList.Count - 1; i >= 0; i--)
                    if (scr.partList[i] != null)
                        RayfireMan.DestroyGo (scr.partList[i].gameObject);
                scr.partList.Clear();
            }
        }
        
        // Destroy clusters
        public static void DestroyClusters (RayfireRigidRoot scr)
        {
            for (int i = 0; i < scr.cluster.shards.Count; i++)
                if (scr.cluster.shards[i].cluster != null && 
                    scr.cluster.shards[i].cluster != scr.connect.cluster && 
                    scr.cluster.shards[i].cluster.tm != null)
                    RayfireMan.DestroyGo (scr.cluster.shards[i].cluster.tm.gameObject);
            scr.clusters.Clear();
        }

        /// /////////////////////////////////////////////////////////
        /// Demolition reset
        /// /////////////////////////////////////////////////////////
        
        // Reinit demolished mesh object
        public static void ResetMeshDemolition (RayfireRigid scr)
        {
            // Edit meshes and fragments only if object was demolished
            if (scr.objectType == ObjectType.Mesh)
            {
                // Reset input shatter
                if (scr.reset.mesh != MeshResetType.ReuseInputMesh)
                    scr.meshDemolition.rfShatter = null;
                
                // Reset Meshes
                if (scr.reset.mesh != MeshResetType.ReuseFragmentMeshes)
                    scr.meshes = null;

                // Fragments need to be reused
                if (scr.reset.fragments == FragmentsResetType.Reuse)
                {
                    // Can be reused. Destroyed if can not
                    if (FragmentReuseState (scr) == true)
                        ReuseFragments (scr);
                    else
                        DestroyFragments (scr);
                }
                
                // Destroy fragments
                else if (scr.reset.fragments == FragmentsResetType.Destroy)
                    DestroyFragments (scr);
                
                // Fragments should be kept in scene. Forget about them
                else if (scr.reset.fragments == FragmentsResetType.Preserve)
                    PreserveFragments (scr);
            }
      
            // Activate
            scr.gameObject.SetActive (true);
        }
        
        // Destroy fragments and root
        static void DestroyFragments (RayfireRigid scr)
        {
            // Destroy fragments    
            if (scr.HasFragments == true)
            {
                // Get amount of fragments
                int fragmentNum = scr.fragments.Count (t => t != null);

                // Destroy fragments and root
                for (int i = scr.fragments.Count - 1; i >= 0; i--)
                {
                    if (scr.fragments[i] != null)
                    {
                        // Destroy particles
                        DestroyParticles (scr.fragments[i]);
                        
                        // Destroy fragment
                        scr.fragments[i].gameObject.SetActive (false);
                        RayfireMan.DestroyGo (scr.fragments[i].gameObject);

                        // Destroy root
                        if (scr.fragments[i].rootParent != null)
                        {
                            scr.fragments[i].rootParent.gameObject.SetActive (false);
                            RayfireMan.DestroyGo (scr.fragments[i].rootParent.gameObject);
                        }
                    }
                }
                
                // Nullify
                scr.fragments = null;

                // Subtract amount of deleted fragments
                RayfireMan.inst.advancedDemolitionProperties.currentAmount -= fragmentNum;

                // Destroy descendants
                if (scr.limitations.descendants != null && scr.limitations.descendants.Count > 0)
                {
                    // Get amount of descendants
                    int descendantNum = scr.limitations.descendants.Count (t => t != null);
                    
                    // Destroy fragments and root
                    for (int i = 0; i < scr.limitations.descendants.Count; i++)
                    {
                        if (scr.limitations.descendants[i] != null)
                        {
                            // Destroy fragment
                            scr.limitations.descendants[i].gameObject.SetActive (false);
                            RayfireMan.DestroyGo (scr.limitations.descendants[i].gameObject);

                            // Destroy root
                            if (scr.limitations.descendants[i].rootParent != null)
                            {
                                scr.limitations.descendants[i].rootParent.gameObject.SetActive (false);
                                RayfireMan.DestroyGo (scr.limitations.descendants[i].rootParent.gameObject);
                            }
                        }
                    }
                    
                    // Clear
                    scr.limitations.descendants.Clear();
                    
                    // Subtract amount of deleted fragments
                    RayfireMan.inst.advancedDemolitionProperties.currentAmount -= descendantNum;
                }
            }
        }

        // Destroy particles
        public static void DestroyParticles (RayfireRigid scr)
        {
            // Destroy debris
            if (scr.HasDebris == true)
                for (int d = 0; d < scr.debrisList.Count; d++)
                    if (scr.debrisList[d].hostTm != null)
                    {
                        scr.debrisList[d].hostTm.gameObject.SetActive (false);
                        RayfireMan.DestroyGo (scr.debrisList[d].hostTm.gameObject);
                    }

            // Destroy debris
            if (scr.HasDust == true)
                for (int d = 0; d < scr.dustList.Count; d++)
                    if (scr.dustList[d].hostTm != null)
                    {
                        scr.dustList[d].hostTm.gameObject.SetActive (false);
                        RayfireMan.DestroyGo (scr.dustList[d].hostTm.gameObject);
                    }
        }
        
        // Fragments need and can be reused
        static void ReuseFragments (RayfireRigid scr)
        {
            // Sub amount
            RayfireMan.inst.advancedDemolitionProperties.currentAmount -= scr.fragments.Count;
            
            // Activate root
            if (scr.rootChild != null)
            {
                scr.rootChild.gameObject.SetActive (false);
                scr.rootChild.position = scr.transForm.position;
                scr.rootChild.rotation = scr.transForm.rotation;
            }

            // Reset fragments tm
            for (int i = scr.fragments.Count - 1; i >= 0; i--)
            {
                // Destroy particles
                DestroyParticles (scr.fragments[i]);
                
                scr.fragments[i].transForm.localScale = scr.fragments[i].physics.initScale;
                scr.fragments[i].transForm.position = scr.transForm.position + scr.pivots[i];
                scr.fragments[i].transForm.rotation = Quaternion.identity;

                // Reset activation TODO check if it was Kinematic
                if (scr.fragments[i].activation.activated == true)
                    scr.fragments[i].simulationType = SimType.Inactive;
                
                // Reset fading
                if (scr.fragments[i].fading.state >= 1)
                    ResetFade(scr.fragments[i]);
                
                // Reset rigid props
                Reset (scr.fragments[i]);
            }

            // Clear descendants
            scr.limitations.descendants.Clear();
        }
        
        // Preserve Fragments
        static void PreserveFragments (RayfireRigid scr)
        {
            scr.fragments = null;
            scr.rootChild = null;
            scr.limitations.descendants.Clear();
        }
          
        // Reinit demolished mesh object
        static void ResetClusterDemolition (RayfireRigid scr)
        {
            if (scr.objectType == ObjectType.ConnectedCluster || scr.objectType == ObjectType.NestedCluster)
            {
                RFBackupCluster.ResetRigidCluster (scr);
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Reuse state
        /// /////////////////////////////////////////////////////////          
        
        // Check fragments reuse state
        static bool ObjectReuseState (RayfireRigid scr)
        {
            // Mesh Root reset
            if (scr.objectType == ObjectType.MeshRoot)
                return true;
            
            // Excluded from sim
            if (scr.physics.exclude == true)
            {
                Debug.Log ("Demolished " + scr.objectType.ToString() + " reset not supported yet.");
                return false;
            }
            
            // Not mesh object type
            if (scr.objectType == ObjectType.Mesh 
                || scr.objectType == ObjectType.ConnectedCluster
                || scr.objectType == ObjectType.NestedCluster)
                return true;
            
            // Object can be reused
            return false;
        }
                
        // Check fragments reuse state
        static bool FragmentReuseState (RayfireRigid scr)
        {
            // Do not reuse reference demolition
            if (scr.demolitionType == DemolitionType.ReferenceDemolition)
                return false;
            
            // Fragments list null or empty
            if (scr.HasFragments == false)
                return false;

            // One of the fragment null
            if (scr.fragments.Any (t => t == null))
                return false;
            
            // One of the fragment going to be destroyed TODO make reusable
            if (scr.fragments.Any (t => t.reset.toBeDestroyed == true))
                return false;
            
            // One of the fragment demolished TODO make reusable
            if (scr.fragments.Any (t => t.limitations.demolished == true))
                return false;
  
            // Fragments can be reused
            return true;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Other
        /// /////////////////////////////////////////////////////////      
        
        // Restore transform or initial
        static void RestoreTransform (RayfireRigid scr)
        {
            // Restore tm
            scr.transForm.localScale = scr.physics.initScale;
            scr.transForm.position   = scr.physics.initPosition;
            scr.transForm.rotation   = scr.physics.initRotation;
            scr.physics.velocity     = Vector3.zero;
            
            // Restore rigidbody TODO save initial velocity into vars and reset to them
            if (scr.physics.rigidBody != null)
            {
                scr.physics.rigidBody.velocity        = Vector3.zero;
                scr.physics.rigidBody.angularVelocity = Vector3.zero;
            }
        }
        
        // Restore rigid properties
        public static void Reset (RayfireRigid scr)
        {
            // Reset caching if it is on
            scr.meshDemolition.StopRuntimeCaching();
            
            scr.physics.Reset();
            scr.activation.Reset();
            if (scr.restriction != null)
                scr.restriction.Reset();
            scr.limitations.Reset();
            scr.meshDemolition.Reset();
            scr.clusterDemolition.Reset();
            scr.fading.Reset();
            if (scr.reset.damage == true)
                scr.damage.Reset();
            
            // Set physical simulation type. Important. Should after collider material define
            RFPhysic.SetSimulationType (scr.physics.rigidBody, scr.simulationType, scr.objectType, scr.physics.useGravity, scr.physics.solverIterations);
            
            // Set sleeping state TODO
            if (scr.simulationType == SimType.Sleeping)
            {
                scr.physics.velocity                  = Vector3.zero;
                scr.physics.rigidBody.velocity        = Vector3.zero;
                scr.physics.rigidBody.angularVelocity = Vector3.zero;
                scr.physics.rigidBody.Sleep();
            }
        }
    }
}