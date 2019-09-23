using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour
{
    //Stats and shit
    int levelGlobal; //Maybe like a global calculated level based on current progress of all of the systems

    enum SystemNames
    {
        Mining,
        Fishing,
        Cooking,
        Woodcutting,
        Farming,
        Smithing,
        Firemaking
    }
    struct SystemItem
    {
        SystemNames System;
        int level;
        int exp;
    }

    SystemItem[] Systems; //Need to store each system for the player


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
