using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    public class MinigameBar : MinigameMaster
    {
        public Vector2 range;
        bool directionLeft;
        bool inside = false;
        public float barSpeed = 1;
        private RectTransform brt, grt;

        void Start()
        {
            //randomize starting position of goal and paddle
            brt = bar.GetComponent<RectTransform>();
            grt = goal.GetComponent<RectTransform>();

            brt.localPosition = new Vector3(Random.Range(-45, 45), brt.localPosition.y, brt.localPosition.z);
            grt.localPosition = new Vector3(Random.Range(-45, 45), grt.localPosition.y, grt.localPosition.z);
        }

        // Update is called once per frame
        void Update()
        {
            float curSpeed = barSpeed * Time.deltaTime;
            //move left/right
            brt.localPosition = new Vector3(brt.localPosition.x + (directionLeft ? -curSpeed : +curSpeed), brt.localPosition.y, brt.localPosition.z);
            //Check if bar hits end
            if (brt.localPosition.x <= range.x)
            {
                directionLeft = false;
            }
            else if (brt.localPosition.x >= range.y)
            {
                directionLeft = true;
            }
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
                }
            }

            if (brt.localPosition.x >= (grt.localPosition.x - grt.rect.width / 2) && brt.localPosition.x <= (grt.localPosition.x + grt.rect.width / 2))
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
