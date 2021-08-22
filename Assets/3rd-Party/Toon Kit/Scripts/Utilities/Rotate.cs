using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonKitOS
{
    public class Rotate : MonoBehaviour
    {
        public Vector3 speed = new Vector3(360f, 360f, 360f);
        
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(speed * Time.deltaTime, Space.World);
        }
    }
}