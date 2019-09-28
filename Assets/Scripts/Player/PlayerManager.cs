using Project.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Player
{
	public class PlayerManager : MonoBehaviour
	{
		[Header("Data")]
		[SerializeField]
		private float speed = 4;

		[Header("Class References")]
		[SerializeField]
		private NetworkIdentity networkIdentity;

		// Update is called once per frame
		void Update()
		{
			if (networkIdentity.IsControlling())
			{
				CheckMovement();
			}
		}

		private void CheckMovement()
		{
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			transform.position += new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
		}
	}
}

