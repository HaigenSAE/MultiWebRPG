using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    public class MinigameRadial : MinigameMaster
    {
        public int range;
        public int goalRad;
        bool directionLeft;
        public bool inside = false;
        public float barSpeed = 1;
        private float zRot;
        private RectTransform brt, grt;

        void Start()
        {
            //randomize starting position of goal and paddle
            brt = bar.GetComponent<RectTransform>();
            grt = goal.GetComponent<RectTransform>();

            brt.localRotation = Quaternion.Euler(0,0,Random.Range(0,360));
            grt.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        // Update is called once per frame
        void Update()
        {
            float curSpeed = barSpeed * Time.deltaTime;
            //move left/right
            
            brt.transform.Rotate(0, 0, directionLeft ? -curSpeed : +curSpeed);
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
            
            if (brt.localEulerAngles.z >= grt.localEulerAngles.z - goalRad && brt.localEulerAngles.z <= grt.localEulerAngles.z + goalRad)
            {
                inside = true;
            }
            else
            {
                inside = false;
            }
        }
    }

}
