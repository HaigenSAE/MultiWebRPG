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

        public Text userExists;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ni.SetSocketReference(GetComponent<NetworkClient>());
        }

        public void tryLogin(Text text)
        {
            LoginData lData = new LoginData();
            lData.loginID = text.text.ToString();
            Debug.Log("Trying to login with: " + lData.loginID);
            SceneManager.LoadScene("Kitchen");
            ni.GetSocket().Emit("loginClient", new JSONObject(JsonUtility.ToJson(lData)));
            Destroy(ni);
            Destroy(this);
        }

        public void register(Text text)
        {
            LoginData lData = new LoginData();
            lData.regoID = text.text.ToString();
            ni.GetSocket().Emit("registerClient", new JSONObject(JsonUtility.ToJson(lData)));
            Debug.Log("Creating new user with: " + lData.regoID);
        }

        public void successfulRegistration()
        {
            SceneManager.LoadScene("Kitchen");
            Destroy(ni);
            Destroy(this);
        }

        public void alreadyExists()
        {
            userExists.gameObject.SetActive(true);
        }
    }

    class LoginData
    {
        public string loginID;
        public string regoID;
    }
}


