/*
ECX UTILITY SCRIPTS
Isometric Camera Controller
Last updated: July 18, 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.InputSystem;   // uncomment this if using Unity's new InputSystem
using EcxUtilities;

namespace EcxUtilities {
    public class CameraControllerIsometric : MonoBehaviour {

        // public InputAction playerInputActions;
        public Camera cameraIso;
        public float moveSpeed = 15f;
        public float zoomSpeed = 15f;
        private float moveMultiplier = 1f;
        private float zoomMultiplier = 1f;
        private float startViewSize = 7;
        private float targetViewSize;
        private Vector2 cameraMoveVector;
        private Vector3 cameraTarget;

        private void Awake() {
            // playerInputActions.performed += Move;
            if(!cameraIso) {
                cameraIso = GetComponent<Camera>();
            }
            targetViewSize = cameraIso.orthographicSize;
        }

        private void Update() {
            MoveCamera();
            ZoomCamera();
        }
        
        private void MoveCamera() {
            cameraTarget.x = Mathf.Clamp(transform.position.x + (cameraMoveVector.x), -100, 100);
            cameraTarget.y = transform.position.y;  
            cameraTarget.z = Mathf.Clamp(transform.position.z + (cameraMoveVector.y * 1.5f), -100, 100);  // bump this by a bit to even out movement speed

            moveMultiplier = Mathf.Clamp(cameraIso.orthographicSize / startViewSize, 0.75f, 3f);  // makes it so movement speed is faster or slower depending on zoom

            transform.position = Vector3.Lerp(transform.position, cameraTarget, Time.deltaTime * moveSpeed * moveMultiplier);
        }

        private void ZoomCamera() {
            if(targetViewSize != cameraIso.orthographicSize) {
                cameraIso.orthographicSize = Mathf.Lerp(cameraIso.orthographicSize, targetViewSize, Time.deltaTime * zoomSpeed * zoomMultiplier);
            }
        }

        // Uncomment the methods below if using Unity's new InputSystem
        // public void OnMove(InputValue value) {
        //     cameraMoveVector = value.Get<Vector2>();
        // }

        // public void OnScrollWheel(InputValue value) {
        //     float scrollValue = value.Get<Vector2>().y;
        //     targetViewSize = Mathf.Clamp(cameraIso.orthographicSize + (scrollValue * -1f), 2, 23);
        //     zoomMultiplier = Mathf.Clamp(scrollValue % 40, 1f, 3f);
        // }


    } // end CameraControllerIsometric
} // end namespace UnityUtilities