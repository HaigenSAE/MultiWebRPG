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
        public Text userLogged;
        public Button loginButton;
        public Button regoButton;
        public Text loginUserText;
        public Text loginPassText;
        public Text regoUserText;
        public Text regoPassText;
        public Text regoUsernameText;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ni.SetSocketReference(GetComponent<NetworkClient>());
        }

        private void Start()
        {
            loginButton.onClick.AddListener(() => tryLogin(loginUserText, loginPassText));
            regoButton.onClick.AddListener(() => register(regoUserText, regoPassText, regoUsernameText));
        }

        public void tryLogin(Text userText, Text passText)
        {
            if (passText.text.Length >= 6)
            {
                LoginData lData = new LoginData();
                lData.loginID = userText.text.ToString();
                lData.password = passText.text.ToString();
                loginButton.gameObject.SetActive(false);
                regoButton.gameObject.SetActive(false);
                Debug.Log("Trying to login with: " + lData.loginID);
                ni.GetSocket().Emit("loginClient", new JSONObject(JsonUtility.ToJson(lData)));
            }
            else
            {
                userExists.text = "Password must be at least 6 characters";
                userExists.gameObject.SetActive(true);
            }
               
        }

        public void register(Text userText, Text passText, Text usernameText)
        {
            if (passText.text.Length >= 6)
            {
                if(usernameText.text.Length > 3)
                {
                    if (userText.text.Length >= 8)
                    {
                        LoginData lData = new LoginData();
                        lData.regoID = userText.text.ToString();
                        lData.password = passText.text.ToString();
                        lData.username = usernameText.text.ToString();
                        loginButton.gameObject.SetActive(false);
                        regoButton.gameObject.SetActive(false);
                        ni.GetSocket().Emit("registerClient", new JSONObject(JsonUtility.ToJson(lData)));
                        Debug.Log("Creating new user with: " + lData.regoID);
                    }
                    else
                    {
                        userExists.text = "Please enter a valid email address";
                        userExists.gameObject.SetActive(true);
                    }                    
                }
                else
                {
                    userExists.text = "Username must be at least 4 characters";
                    userExists.gameObject.SetActive(true);
                }             
            }
            else
            {
                userExists.text = "Password must be at least 6 characters";
                userExists.gameObject.SetActive(true);
            }
        }

        public void successfulPass()
        {
            SceneManager.LoadScene("Kitchen");
            Destroy(ni);
            Destroy(this);
        }

        public void alreadyExists()
        {
            userExists.text = "User already exists";
            userExists.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            regoButton.gameObject.SetActive(true);
        }

        public void invalidEmail()
        {
            userExists.text = "Email address is invalid";
            userExists.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            regoButton.gameObject.SetActive(true);
        }

        public void alreadyLoggedIn()
        {
            userLogged.text = "User already logged in";
            userLogged.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            regoButton.gameObject.SetActive(true);
        }

        public void userNotFound()
        {
            userLogged.text = "User not found";
            userLogged.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            regoButton.gameObject.SetActive(true);
        }

        public void incorrectPassword()
        {
            userLogged.text = "Password Incorrect";
            userLogged.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            regoButton.gameObject.SetActive(true);
        }
    }

    class LoginData
    {
        public string loginID;
        public string regoID;
        public string username;
        public string password;
    }
}


