using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonKitOS
{
    public class Pan : MonoBehaviour
    {
        [SerializeField]
        Vector3 speed;

        // Update is called once per frame
        void Update()
        {
            transform.Translate(speed * Time.deltaTime);
        }
    }
}

