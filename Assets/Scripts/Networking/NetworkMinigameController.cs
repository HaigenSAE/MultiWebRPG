using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{
    public class NetworkMinigameController : MonoBehaviour
    {

        public void successfulMinigame(string minigameName, NetworkIdentity networkIdRef)
        {
            Player player = new Player();
            player.minigameWon = minigameName;
            networkIdRef.GetSocket().Emit("successfulMinigame", new JSONObject(JsonUtility.ToJson(player)));
            Debug.Log("Emit sent");
        }
    }
}


