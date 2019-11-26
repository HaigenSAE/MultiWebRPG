using Project.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Player
{
	public class PlayerManager : MonoBehaviour
	{
		[Header("Data")]
		[SerializeField]
		private float speed = 4;
        public bool mouseInput;
        public bool isInInteraction;
        private bool doMove;
        public Vector3 destination;
        public PlayerStats playerStats;

        [Header("Class References")]
		[SerializeField]
		public NetworkIdentity networkIdentity;

        private void Start()
        {
            destination = transform.position;
            playerStats = GetComponent<PlayerStats>();
        }

        // Update is called once per frame
        void Update()
		{
			if (networkIdentity.IsControlling())
			{
				CheckMovement();
			}
        }
        
        public void MinigameCompleted(string skillName)
        {
            Debug.Log("talking to networkcontroller");
            GetComponent<NetworkMinigameController>().successfulMinigame(skillName); 
        }

        private void CheckMovement()
		{
            if (Input.GetMouseButton(0))
            {
                //Raycasting
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    NavMeshPath path = new NavMeshPath();
                    NavMeshHit navHit = new NavMeshHit();
                    //Don't try to move if we didn't hit a navmesh
                    //We do want to move if we hit an interactable and we want to begin the interaction, we also don't want to move once inside the interaction.
                    if (!isInInteraction)
                    {
                        if (NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, path))
                        {
                            destination = path.corners[path.corners.Length - 1];
                            destination = new Vector3(destination.x, transform.position.y, destination.z);
                            doMove = true;
                        }
                        //If we hit the interactable object
                        if (!mouseInput && hit.transform.gameObject.tag == "Interactable")
                        {
                            //Get the closest possible point we can travel to
                            NavMesh.SamplePosition(hit.point, out navHit, 5.0f, NavMesh.AllAreas);
                            destination = navHit.position;
                            destination = new Vector3(destination.x, transform.position.y, destination.z);
                            doMove = true;
                            //Move to position outside of table and bring up UI
                            hit.transform.GetComponent<Interactable>().OpenInteractable(gameObject);
                            isInInteraction = true;
                        }
                    }
                }
                //storing mouse input, we only want interactables clicked on once, not ~30-60 times per second
                mouseInput = true;
            }
            else
            {
                //Reset mouse input
                mouseInput = false;
            }

            if(doMove)
            {
                //Actual movement, nice lerpy movement
                transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * speed);
                if(Vector3.Distance(transform.position, destination) <= 0.1f)
                {
                    doMove = false;
                }
            }
        }
	}
}

