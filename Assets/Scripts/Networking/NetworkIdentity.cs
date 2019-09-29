using Project.Utility.Attributes;
using UnitySocketIO.SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{
	public class NetworkIdentity : MonoBehaviour
	{
		[Header("Helpful Values")]
		[SerializeField]
		[GreyOut]
		private string id;
		[SerializeField]
		[GreyOut]
		private bool isControlling;

		private BaseSocketIO socket;

		public void Awake()
		{
			isControlling = false;
		}

		public void SetControllerID(string ID)
		{
			id = ID;
			isControlling = (NetworkClient.ClientID == ID) ? true : false; //Check incoming ID vs the one we have saved from server
		}

		public void SetSocketReference(BaseSocketIO Socket)
		{
			socket = Socket;
		}

		public string GetID()
		{
			return id;
		}

		public bool IsControlling()
		{
			return isControlling;
		}

		public BaseSocketIO GetSocket()
		{
			return socket;
		}
	}
}
