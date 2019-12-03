using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using Project.Utility;
using System;

namespace Project.Networking
{
    public class NetworkClient : SocketIOComponent
    {
        [Header("Network Client")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefab;

        public NetworkLogin networkLogin;
        public NetworkChatbox networkChatbox;

        public GameObject localPlayerObj;

        public static string ClientID { get; private set; }

        Dictionary<string, NetworkIdentity> serverObjects;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            Initialize();
            SetupEvents();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            if(ClientID != null)
            {
                if (serverObjects.ContainsKey(ClientID))
                    localPlayerObj = serverObjects[ClientID].gameObject;
            }
        }

        private void Initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        private void SetupEvents()
        {
            On("open", (E) => 
            {
                Debug.Log("Connection made to server");
            });

            On("registered", (E) =>
            {
                ClientID = E.data["id"].ToString().RemoveQuotes();
                Debug.LogFormat("Our Client's ID ({0})", ClientID);
                networkLogin.successfulPass();
                Emit("enterGame");
                
            });

            On("loggedIn", (E) =>
            {
                ClientID = E.data["id"].ToString().RemoveQuotes();
                Debug.LogFormat("Our Client's ID ({0})", ClientID); 
                networkLogin.successfulPass();
                Emit("enterGame");
            });

            On("spawn", (E) =>
            {
                //Handle all spawning of players
                //Passed Data
                string id = E.data["id"].ToString().RemoveQuotes();
                GameObject[] spawns = GameObject.FindGameObjectsWithTag("Respawn");
                Transform initSpawn = spawns[UnityEngine.Random.Range(0, spawns.Length)].transform;
                GameObject go = Instantiate(playerPrefab, initSpawn.position, initSpawn.rotation, networkContainer);
                go.name = string.Format("Player ({0})", id);
                go.GetComponent<Project.Player.PlayerManager>().username = E.data["username"].ToString().RemoveQuotes();
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);
                serverObjects.Add(id, ni);
            });

            On("disconnected", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = serverObjects[id].gameObject;
                Destroy(go); //Remove from game
                serverObjects.Remove(id); //Remove from memory
            });

            On("updatePosition", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();
                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                float z = E.data["position"]["z"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, z);
            });

            On("loadData", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = serverObjects[id].gameObject;
                go.GetComponent<Project.Player.PlayerManager>().username = E.data["username"].ToString().RemoveQuotes();
                PlayerStats ps = go.GetComponent<PlayerStats>();
                for (int i = 0; i < 6; i++)
                {
                    ps.skills[i].skillName = E.data["playerStats"]["skills"[i]]["skillName"].ToString();
                    ps.skills[i].curLevel = Mathf.RoundToInt(E.data["playerStats"]["skills"[i]]["curLevel"].f);
                    ps.skills[i].curExp = Mathf.RoundToInt(E.data["playerStats"]["skills"[i]]["curExp"].f);
                }
            });

            On("successMinigame", (E) =>
            {
                Debug.Log("Success Called From Server");
                string id = E.data["id"].ToString().RemoveQuotes();
                GameObject go = serverObjects[id].gameObject;
                PlayerStats ps = go.GetComponent<PlayerStats>();
                Debug.Log(go.name);
                Debug.Log(E.data["minigameWon"]);
                foreach (Skill skill in ps.skills)
                {
                    if (skill.skillName == E.data["minigameWon"].str)
                    {
                        skill.IncreaseExp((int)E.data["expAward"].f);
                        Debug.Log("Awarded exp to: " + skill.skillName);
                    }
                }
                go.GetComponent<Project.Player.PlayerManager>().SaveData();
            });

            On("alreadyExists", (E) =>
            {
                AlreadyExists();
                Debug.Log("already exists");
            });

            On("alreadyLoggedIn", (E) =>
            {
                AlreadyLoggedIn();
                Debug.Log("AlreadyLoggedIn");
            });

            On("userNotFound", (E) =>
            {
                UserNotFound();
                Debug.Log("UserNotFound");
            });

            On("incorrectPassword", (E) =>
            {
                PasswordIncorrect();
                Debug.Log("PasswordIncorrect");
            });

            On("receiveChatMessage", (E) =>
            {
                ReceiveChatMessage(E.data.str);
            });
        }

        public void AlreadyExists()
        {
            if(networkLogin)
            {
                networkLogin.alreadyExists();
            }
        }

        public void AlreadyLoggedIn()
        {
            if (networkLogin)
            {
                networkLogin.alreadyLoggedIn();
            }
        }

        public void UserNotFound()
        {
            if (networkLogin)
                networkLogin.userNotFound();            
        }

        public void PasswordIncorrect()
        {
            if (networkLogin)
                networkLogin.incorrectPassword();
        }

        public void ReceiveChatMessage(string message)
        {
            if (networkChatbox)
                networkChatbox.ReceiveMessage(message);
        }
    }

    [Serializable]
    public class Player
    {
        public string id;
        public Position position;
        public PStats playerStats;
        public string minigameWon;
        public string username;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class PStats
    {
        public List<SkillStats> skills;
    }

    [Serializable]
    public class SkillStats
    {
        public string skillName;
        public int curLevel = 1;
        public int curExp;
    }

}
