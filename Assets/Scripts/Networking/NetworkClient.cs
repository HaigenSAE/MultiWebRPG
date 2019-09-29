using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using Project.Utility;
using System;

namespace Project.Networking
{
	public class NetworkClient : SocketIOController
	{
		[Header("Network Client")]
		[SerializeField]
		private Transform networkContainer;
		[SerializeField]
		private GameObject playerPrefab;

		public static string ClientID { get; private set; }

		private Dictionary<string, NetworkIdentity> serverObjects;

		// Start is called before the first frame update
		public void Start()
		{
			//base.Start();
			Initialize();
			SetupEvents();
		}

		// Update is called once per frame
		public void Update()
		{
			//base.Update();
		}

		private void Initialize()
		{
			serverObjects = new Dictionary<string, NetworkIdentity>();
		}

		private void SetupEvents()
		{
			On("open", (E) => {
				Debug.Log("Connection made to server");
			});

			On("register", (E) =>
			{
				ClientID = E.data["id"].ToString().RemoveQuotes();
				Debug.LogFormat("Our Client's ID ({0})", ClientID);
			});

			On("spawn", (E) =>
			{
				//Handle all spawning of players
				//Passed Data
				string id = E.data["id"].ToString().RemoveQuotes();

				GameObject go = Instantiate(playerPrefab, networkContainer);
				go.name = string.Format("Player ({0})", id);
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
		}

	}

	[Serializable]
	public class Player
	{
		public string id;
		public Position position;
	}

	[Serializable]
	public class Position
	{
		public float x;
		public float y;
		public float z;
	}

}
