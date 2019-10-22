using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Player
{
    public class MinigamePanel : MonoBehaviour
    {
        public Button backButton;
        public GameObject owner;

        // Start is called before the first frame update
        void Start()
        {
            backButton = GetComponentInChildren<Button>();
            backButton.onClick.AddListener(BackPressed);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BackPressed()
        {
            //Exit minigame
            owner.GetComponent<Interactable>().CloseInteractable();
            Destroy(gameObject);
        }
    }

}
