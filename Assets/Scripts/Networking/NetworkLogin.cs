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
        public Button loginButton;
        public Button regoButton;
        public Text loginUserText;
        public Text loginPassText;
        public Text regoUserText;
        public Text regoPassText;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ni.SetSocketReference(GetComponent<NetworkClient>());
        }

        private void Start()
        {
            loginButton.onClick.AddListener(() => tryLogin(loginUserText, loginPassText));
            regoButton.onClick.AddListener(() => tryLogin(regoUserText, regoPassText));
        }

        public void tryLogin(Text userText, Text passText)
        {
            LoginData lData = new LoginData();
            lData.loginID = userText.text.ToString();
            lData.password = passText.text.ToString();
            Debug.Log("Trying to login with: " + lData.loginID);
            SceneManager.LoadScene("Kitchen");
            ni.GetSocket().Emit("loginClient", new JSONObject(JsonUtility.ToJson(lData)));
            Destroy(ni);
            Destroy(this);
        }

        public void register(Text userText, Text passText)
        {
            LoginData lData = new LoginData();
            lData.regoID = userText.text.ToString();
            lData.password = passText.text.ToString();
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
        public string password;
    }
}


