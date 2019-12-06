using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Player
{
    public class PlayerStretch : MonoBehaviour
    {
        public GameObject root;
        public Vector3 newDest;

        public float factor = 1.0f;

        void Start()
        {
            root = transform.parent.gameObject;
            SetPos(transform.position, transform.position);
        }

        void Update()
        {
            if(!root.GetComponent<PlayerManager>().isChatFocused)
            {
                if (Vector3.Distance(root.GetComponent<PlayerManager>().destination, transform.position) > 1)
                {
                    newDest = root.GetComponent<PlayerManager>().destination;
                }
                else
                {
                    newDest = transform.position;
                }
                SetPos(transform.position, newDest);
            } 
        }

        void SetPos(Vector3 start, Vector3 end)
        {
            var dir = end - start;
            var mid = (dir) / 2f + start;
            //transform.position = mid;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            Vector3 scale = transform.localScale;
            scale.y = Mathf.Clamp(dir.magnitude / 2 * factor, 1, 100);
            transform.localScale = scale;
        }
    }
}

