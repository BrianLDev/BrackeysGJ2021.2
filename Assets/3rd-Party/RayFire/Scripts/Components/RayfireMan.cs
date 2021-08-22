using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RayFire
{
    [DisallowMultipleComponent]
    [AddComponentMenu ("RayFire/Rayfire Man")]
    [HelpURL ("https://rayfirestudios.com/unity-online-help/components/unity-man-component/")]
    public class RayfireMan : MonoBehaviour
    {
        public static RayfireMan inst;
        
        [Header ("  Gravity")]
        [Space (3)]
        
        [Tooltip("Sets custom gravity for simulated objects.")]
        public bool setGravity = false;
        [Space (2)]
        
        [Tooltip("Custom gravity multiplier.")]
        [Range (0f, 1f)] public float multiplier = 1f;

        [Header ("  Physics")]
        [Space (3)]
        
        public RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;
        [Space (2)]
        [Range (0f, 1f)] public float colliderSize = 0.05f;
        public static float colliderSizeStatic = 0.05f;
        
        [Header ("  Collision Detection")]
        [Space (3)]
        
        [FormerlySerializedAs ("collisionDetection")] // 1.33
        public CollisionDetectionMode meshCollision = CollisionDetectionMode.ContinuousDynamic;
        [Space (2)]
        public CollisionDetectionMode clusterCollision = CollisionDetectionMode.Discrete;
        
        [Header ("  Materials")]
        [Space (3)]

        [Tooltip("Minimum mass value which will be assigned to simulated object if it's mass calculated by it's volume and density will be less than this value.")]
        [Range (0f, 1f)] public float minimumMass = 0.1f;
        [Space (2)]
        
        [Tooltip("Maximum mass value which will be assigned to simulated object if it's mass calculated by it's volume and density will be higher than this value.")]
        [Range (0f, 400f)] public float maximumMass = 400f;
        [Space (2)]
        
        [Tooltip("List of hardcoded materials with predefined simulation and demolition properties.")]
        public RFMaterialPresets materialPresets = new RFMaterialPresets();

        [Header ("  Activation")]
        [Space (3)]
        
        [Tooltip("Global Solidity multiplier. Affect solidity of all simulated objects.")]
        public GameObject parent;

        [Header ("  Demolition")]
        [Space (3)]

        [Tooltip("Global Solidity multiplier. Affect solidity of all simulated objects.")]
        [Range (0f, 5f)] public float globalSolidity = 1f;
        [Space (2)]
        
        [Tooltip ("Demolition time quota in milliseconds. Allows to prevent demolition at " +
                  "the same frame if there was already another demolition " + 
                  "at the same frame and it took more time than Time Quota value.")]
        [Range (0f, 0.1f)] public float timeQuota = 0.033f;
        [Space (2)]
        
        public RFManDemolition advancedDemolitionProperties = new RFManDemolition();

        [Header ("  Pooling")]
        [Space (3)]
        
        [Tooltip("")]
        public RFPoolingFragment fragments = new RFPoolingFragment();
        
        [Space (2)]
        public RFPoolingParticles particles = new RFPoolingParticles();

        [Header ("  About")]
        [Space (3)]
        
        public static int buildMajor = 1;
        public static int buildMinor = 42;

        // Hidden
        [HideInInspector] public Transform       transForm;
        [HideInInspector] public List<string>    layers;
        [HideInInspector] public RFStorage       storage;
        [HideInInspector] public float           maxTimeThisFrame;
        [HideInInspector] public List<RFUny>     unyList = new List<RFUny>();
        public                   HashSet<string> layersHash;
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        // Awake
        void Awake()
        {
            // Set static instance
            SetInstance();
        }

        // Late Update
        void LateUpdate()
        {
            maxTimeThisFrame = 0f;
        }

        // Set instance
        void SetInstance()
        {
            // Set new static instance
            if (inst == null)
            {
                inst = this;
            }

            // Static instance not defined
            if (inst != null)
            {
                // Instance is this mono
                if (inst == this)
                {
                    // Set vars
                    SetVariables();

                    // Start pooling objects for fragments
                    SetPooling();

                    // Create storage and stat root check coroutine
                    SetStorage();
                }

                // Instance is not this mono
                if (inst != this)
                {
                    Destroy (gameObject);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Enable/Disable
        /// /////////////////////////////////////////////////////////
        
        // Disable
        void OnDisable()
        {
            fragments.inProgress = false;
            particles.inProgress = false;
            storage.inProgress   = false;
        }

        // Activation
        void OnEnable()
        {
            if (gameObject.activeSelf == true)
            {
                // Continue pooling
                SetPooling();
                
                // Continue storage check
                SetStorage();
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////

        // Set vars
        void SetVariables()
        {
            // Get components
            transForm = GetComponent<Transform>();

            // Reset amount
            advancedDemolitionProperties.currentAmount = 0;

            // Set gravity
            SetGravity();

            // Set Physic Materials if needed
            materialPresets.SetMaterials();

            // Set all layers and tags
            SetLayers();

            colliderSizeStatic = colliderSize;
        }

        // Set gravity
        void SetGravity()
        {
            if (setGravity == true)
                Physics.gravity = -9.81f * multiplier * Vector3.up;
        }

        /// /////////////////////////////////////////////////////////
        /// Other
        /// /////////////////////////////////////////////////////////
        
        // Create RayFire manager if not created
        public static void RayFireManInit()
        {
            if (inst == null)
            {
                GameObject rfMan = new GameObject ("RayFireMan");
                inst = rfMan.AddComponent<RayfireMan>();
            }

            EditorCreate();
        }

        // Set instance ops for editor creation
        static void EditorCreate()
        {
            if (Application.isPlaying == false)
            {
                inst.SetInstance();
            }
        }

        // Set list of layers and tags
        void SetLayers()
        {
            // Set layers list
            layers     = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName (i);
                if (layerName.Length > 0)
                    layers.Add (layerName);
            }
            layersHash = new HashSet<string>(layers);
        }
        
        // Max fragments amount check
        public static bool MaxAmountCheck
        {
            get { return inst.advancedDemolitionProperties.currentAmount < inst.advancedDemolitionProperties.maximumAmount; }
        }

        /// /////////////////////////////////////////////////////////
        /// Pooling
        /// /////////////////////////////////////////////////////////

        // Enable objects pooling for fragments                
        void SetPooling()
        {
            // Create pool root
            fragments.CreatePoolRoot (transform);

            // Create pool instance
            fragments.CreateInstance (transform);

            // Pooling. Mot in editor
            if (Application.isPlaying == true && fragments.enable == true && fragments.inProgress == false)
                StartCoroutine (fragments.StartPoolingCor (transForm));

            // Create pool root
            particles.CreatePoolRoot (transform);

            // Create pool instance
            particles.CreateInstance (transform);

            // Pooling. Mot in editor
            if (Application.isPlaying == true && particles.enable == true && particles.inProgress == false)
                StartCoroutine (particles.StartPoolingCor (transForm));
        }
        
        /// /////////////////////////////////////////////////////////
        /// Storage
        /// /////////////////////////////////////////////////////////
        
        // Create storage root
        void SetStorage()
        {
            // Create
            if (storage == null)
                storage = new RFStorage();
            
            // Create storage if has no
            storage.CreateStorageRoot (transform);
            
            // Start empty root removing coroutine if not running
            if (Application.isPlaying == true && storage.inProgress == false)
                StartCoroutine (storage.StorageCor ());
        }

        /// /////////////////////////////////////////////////////////
        /// Parent
        /// /////////////////////////////////////////////////////////

        // Set root to manager or to the same parent
        public static void SetParentByManager (Transform tm, Transform original, bool noRegister = false)
        {
            if (inst != null && inst.advancedDemolitionProperties.parent == RFManDemolition.FragmentParentType.Manager)
                tm.parent = inst.storage.storageRoot;
            else
                tm.parent = original.parent;

            // Register in storage
            if (noRegister == false)
                inst.storage.Register (tm);
        }
        
        // Set root to manager or to the same parent
        public static void SetParentToManager (Transform tm)
        {
           tm.parent = inst.storage.storageRoot;
           
           // Register in storage
           inst.storage.Register (tm);
        }
        
        // Set root to manager or to the same parent
        public static void SetParentByManager (Transform tm)
        {
            if (inst != null && inst.advancedDemolitionProperties.parent == RFManDemolition.FragmentParentType.Manager)
                tm.parent = inst.storage.storageRoot;
            
            // Register in storage
            inst.storage.Register (tm);
        }

        /// /////////////////////////////////////////////////////////
        /// Destroy/Deactivate Fragment/Shard
        /// /////////////////////////////////////////////////////////

        // Check if fragment is the last child in root and delete root as well
        public static void DestroyFragment (RayfireRigid scr, Transform tm, float time = 0f)
        {
            // Decrement total amount.
            if (Application.isPlaying == true)
                inst.advancedDemolitionProperties.currentAmount--;
            
            // Deactivate
            scr.gameObject.SetActive (false);
            
            // Destroy
            if (scr.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
                DestroyOp (scr, tm, time);
        }

        public static void DestroyShard (RayfireRigidRoot scr, RFShard shard)
        {
            // Deactivate
            shard.tm.gameObject.SetActive (false);
            
            // Destroy
            if (scr.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
                DestroyGo (shard.tm.gameObject);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Destroy
        /// /////////////////////////////////////////////////////////
        
        // Check if fragment is the last child in root and delete root as well
        public static void DestroyGo (GameObject go)
        {
            Destroy (go);
        }

        // Check if fragment is the last child in root and delete root as well
        static void DestroyOp (RayfireRigid scr, Transform tm, float time = 0f)
        {
            // Set delay
            if (time == 0)
                time = scr.reset.destroyDelay;

            // Object is going to be destroyed. Timer is on
            scr.reset.toBeDestroyed = true;

            // Destroy object
            if (time <= 0f)
                Destroy (scr.gameObject);
            else
                Destroy (scr.gameObject, time);

            // Destroy root
            if (tm != null && tm.childCount == 0)
            {
                // TODO collect root in special roots list, check every 10 seconds and destroy if they are empty
                Destroy (tm.gameObject, time);
            }
        }
    }
}

