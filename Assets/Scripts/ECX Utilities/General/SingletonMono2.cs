/*
ECX UTILITY SCRIPTS
Singleton MonoBehaviour
Last updated: June 18, 2021
*/

using UnityEngine;

namespace EcxUtilities {
    /// <summary>
    /// This singleton is a template that can be inherited by any class that needs global access to only 1 instance
    /// e.g. GameManagers or AudioPlayers.
    /// Named "SingletonMono" to differentiate from the future DOTS/ECS version which won't use MonoBehaviour.
    /// Note - to use multiple singletons in a game (e.g. Audio & UI), create a copy of this script and rename the class to something slightly different (e.g. SingtonMono2).
    /// Declaration syntax:     
    ///     public class AudioManager : SingletonMono<AudioManager>
    /// </summary>
    public class SingletonMono2<T> : MonoBehaviour  where T : MonoBehaviour{   
        
        public static bool organizeManagerGameObj = true;
        public static bool parentDontDestroyOnLoad = true;
        public static bool verboseDebugLog = false;
        private static T m_instance;
        public static T Instance { 
            get {
                // 1) On first access, create the singleton
                if (m_instance == null) {
                    // Find the object and assign it to the private singleton variable (only done once)
                    m_instance = (T)FindObjectOfType(typeof(T) );
                    if (verboseDebugLog)
                        Debug.Log("Initializing singleton of type " + typeof(T).ToString() + ".  Intance = " + m_instance);

                    // Make sure that there aren't multiple singletons 
                    if (FindObjectsOfType(typeof(T)).Length > 1) {  
                        Debug.LogError("Error!  There should only be 1 intance of " + typeof(T).ToString() + " singleton.  Check your scene and remove any duplicates.");
                    }
                    // If somehow the <T> object was not found, create a new one
                    if (m_instance == null) {
                        if (verboseDebugLog)
                            Debug.Log("SingletonMono couldn't find object of type " + typeof(T).ToString()  + ". Creating a new one.");
                        GameObject newGameObj = new GameObject();
                        newGameObj.AddComponent<T>();
                        m_instance = newGameObj.GetComponent<T>();
                    }
                    // Organize the game object under a "Managers" parent if option is checked
                    if (organizeManagerGameObj) {
                        GameObject parent = GameObject.Find("Managers");
                        if (!parent) {
                            if (verboseDebugLog)
                                Debug.Log("Creating new parent manager");
                            parent = new GameObject("Managers");
                        }
                        m_instance.gameObject.transform.parent = parent.transform;
                        if (verboseDebugLog)
                            Debug.Log("Parent of singleton is " + m_instance.gameObject.transform.parent);
                    }
                    // Set the root parent to Don't destroy on load if the option is checked
                    if (parentDontDestroyOnLoad)
                        DontDestroyOnLoad(m_instance.gameObject.transform.root);
                    
                    // Finally, name the singleton and return it
                    m_instance.name = typeof(T).ToString() + " (singleton)";
                    int indexOfDot = m_instance.name.IndexOf(".");
                    m_instance.name = m_instance.name.Substring(indexOfDot+1);
                    return m_instance;
                }
                // 2) In all future cases where the singleton exists, simply return it.
                else {
                    return m_instance;
                }
            } 
            private set {
                // nothing needed here
            }
        }
        
    }
}
