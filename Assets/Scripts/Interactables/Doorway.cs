using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doorway : MonoBehaviour
{
    // Start is called before the first frame update

    public string targetLevel;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Travel()
    {
        //log that PLAYERID is going to SCENE
        SceneManager.LoadScene(targetLevel);
    }
}
