using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatboxPanel : MonoBehaviour
{
    public List<GameObject> panels;


    //animation
    public void OpenPanel()
    {
        foreach(GameObject panel in panels)
        {
            Animator animator = panel.GetComponent<Animator>();
            if(animator != null)
            {
                bool isOpen = animator.GetBool("open");

                animator.SetBool("open", !isOpen);
            }
        }
    }
}
