using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    public class MinigameRadial : MonoBehaviour
    {
        public GameObject goal;
        public GameObject bar;
        public int range;
        public int goalRad;
        bool directionLeft;
        bool inside = false;
        public float barSpeed = 1;
        int fails;

        // Update is called once per frame
        void Update()
        {
            float curSpeed = barSpeed * Time.deltaTime;
            //move left/right
            RectTransform brt = bar.GetComponent<RectTransform>();
            brt.localRotation = Quaternion.Euler(brt.localRotation.x, brt.localRotation.y, brt.localRotation.z + (directionLeft ? -curSpeed : +curSpeed));
            if (Input.GetMouseButtonDown(0))
            {
                //was it inside?
                if (inside)
                {
                    Debug.Log("success!");
                    GetComponentInParent<MinigamePanel>().Success();
                    Destroy(transform.gameObject);
                }
                else
                {
                    Debug.Log("Fail");
                    //fail (if 3 fails, kick out)
                    fails++;
                    if (fails == 3)
                    {
                        //better logic later
                        Destroy(transform.gameObject);
                    }
                    directionLeft = !directionLeft;
                }
            }
            RectTransform grt = goal.GetComponent<RectTransform>();
            if (brt.localRotation.z >= grt.localRotation.z - goalRad && brt.localRotation.z <= grt.localRotation.z + goalRad)
            {
                inside = true;
            }
            else
            {
                inside = false;
            }
        }

        public void Success()
        {

        }
    }

}
