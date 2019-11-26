using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Player
{
    public class MinigamePanel : MonoBehaviour
    {
        public Button backButton;
        public GameObject ownerPlayer;
        public GameObject ownerInteractable;
        public GameObject[] miniGames;
        public GameObject curMinigame;
        public Text title;
        public string minigameSkill;

        // Start is called before the first frame update
        void Start()
        {
            backButton = GetComponentInChildren<Button>();
            backButton.onClick.AddListener(BackPressed);
            curMinigame = Instantiate(miniGames[Random.Range(0, miniGames.Length)], transform);
            curMinigame.GetComponent<MinigameMaster>().playerObj = ownerPlayer;
        }

        public void BackPressed()
        {
            //Exit minigame
            ownerInteractable.GetComponent<Interactable>().CloseInteractable();
            Destroy(gameObject);
        }

        public void Success()
        {
            title.text = "Success!";
            ownerPlayer.GetComponent<PlayerManager>().MinigameCompleted(minigameSkill);
        }
    }

}
