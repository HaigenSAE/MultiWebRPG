using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{
    public class NetworkMinigameController : MonoBehaviour
    {
        NetworkIdentity ni;

        // Start is called before the first frame update
        void Start()
        {
            ni = GetComponent<NetworkIdentity>();
            
        }

        public void successfulMinigame(string minigameName)
        {
            Player player = new Player();
            player.minigameWon = minigameName;
            ni.GetSocket().Emit("successfulMinigame", new JSONObject(JsonUtility.ToJson(player)));
            Debug.Log("Emit sent");
        }
    }
}


