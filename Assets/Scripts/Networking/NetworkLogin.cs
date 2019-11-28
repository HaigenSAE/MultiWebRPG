using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Networking
{
    public class NetworkLogin : MonoBehaviour
    {

        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;

        public NetworkIdentity ni;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ni.SetSocketReference(GetComponent<NetworkClient>());
        }

        public void tryLogin(Text text)
        {
            LoginData lData = new LoginData();
            lData.loginID = text.text;
            Debug.Log("Trying to login with: " + lData.loginID);
            SceneManager.LoadScene("Kitchen");
            ni.GetSocket().Emit("loginClient", new JSONObject(JsonUtility.ToJson(lData)));
            Destroy(ni);
            Destroy(this);
        }

        public void register()
        {
            LoginData lData = new LoginData();
            SceneManager.LoadScene("Kitchen");
            ni.GetSocket().Emit("registerClient", new JSONObject(JsonUtility.ToJson(lData)));
            ni.GetSocket().Emit("enterGame", new JSONObject(JsonUtility.ToJson(lData)));
            Destroy(ni);
            Destroy(this);
        }
    }

    class LoginData
    {
        public string loginID;
    }
}


