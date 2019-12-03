using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Facecamera : MonoBehaviour
{
    public GameObject player;
    public Transform pivot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.gameObject.GetComponent<TextMesh>().text = player.GetComponent<Project.Player.PlayerManager>().username;
        pivot.transform.LookAt(Camera.main.transform);
    }
}
