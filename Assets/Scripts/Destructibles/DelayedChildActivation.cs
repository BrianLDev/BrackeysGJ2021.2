using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedChildActivation : MonoBehaviour {
    [SerializeField] private float delayPerObjectInSeconds = 0.1f;
    [SerializeField] private GameObject[] childGameObjects;

    private void Start() {
        if (childGameObjects.Length > 0)
            StartCoroutine("BeginChildActivation");
        else
            Debug.LogError("Error: missing array of child objects for delayed spawn on " + gameObject.name);
    }

    private IEnumerator BeginChildActivation() {
        for (int i=0; i<childGameObjects.Length; i++) {
            childGameObjects[i].SetActive(true);
            yield return new WaitForSeconds(delayPerObjectInSeconds);
        }
        yield return null;
    }
}
