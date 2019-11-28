using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
    public class Interactable : MonoBehaviour
    {

        public GameObject mg_panelPrefab;
        private GameObject mg_Panel;
        public GameObject playerTrack;

        public void OpenInteractable(GameObject player)
        {
            playerTrack = player;
            Debug.Log("Opening Interaction Menu");
            mg_Panel = Instantiate(mg_panelPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
            mg_Panel.GetComponent<MinigamePanel>().ownerInteractable = gameObject;
            mg_Panel.GetComponent<MinigamePanel>().ownerPlayer = playerTrack;
        }

        public void CloseInteractable()
        {
            playerTrack.GetComponent<PlayerManager>().SaveData();
            playerTrack.GetComponent<PlayerManager>().isInInteraction = false;
        }
    }
}

