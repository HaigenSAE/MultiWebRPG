using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Networking
{
    public class NetworkChatbox : MonoBehaviour
    {
        public int maxMessages = 25;
        public GameObject chatPanel, textObject, inputField;

        NetworkClient nc;
        GameObject player;

        [SerializeField]
        List<Message> messageList = new List<Message>();

        Message curMessage = new Message();

        void Start()
        {
            if(GameObject.FindGameObjectWithTag("NetworkClient") != null)
            {
                nc = GameObject.FindGameObjectWithTag("NetworkClient").GetComponent<NetworkClient>();
                nc.networkChatbox = this;
            }
            
        }

        void Update()
        {
            if(nc != null)
            {
                player = nc.localPlayerObj;
                player.GetComponent<Project.Player.PlayerManager>().isChatFocused = inputField.GetComponent<InputField>().isFocused;
            }
            
        }

        public void ReceiveMessage(string message)
        {
            if (messageList.Count >= maxMessages)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }
            Message newMessage = new Message();
            newMessage.text = message;

            GameObject newTextObject = Instantiate(textObject, chatPanel.transform);

            newMessage.textObject = newTextObject.GetComponent<Text>();

            newMessage.textObject.text = newMessage.text;

            messageList.Add(newMessage);
        }

        public void UpdateChatMessage(InputField input)
        {
            curMessage.text = player.GetComponent<Project.Player.PlayerManager>().username + ": " + input.text;
        }

        public void SendMessageToChat()
        {
            if(curMessage.text != "")
            {
                if (messageList.Count >= maxMessages)
                {
                    Destroy(messageList[0].textObject.gameObject);
                    messageList.Remove(messageList[0]);
                }

                GameObject newTextObject = Instantiate(textObject, chatPanel.transform);
                curMessage.textObject = newTextObject.GetComponent<Text>();
                curMessage.textObject.text = curMessage.text;

                //emit message
                player.GetComponent<NetworkIdentity>().GetSocket().Emit("newChatMessage", new JSONObject(JsonUtility.ToJson(curMessage)));
            }
        }
    }
    public class Message
    {
        public string text;
        public Text textObject;
    }
}




