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

        public float factor = 1.0f;

        [Header("Class References")]
		[SerializeField]
		private NetworkIdentity networkIdentity;

        private void Start()
        {

        }

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
            if (Input.GetMouseButton(0))
            {
                //Raycasting
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    NavMeshPath path = new NavMeshPath();
                    //Don't try to move if we didn't hit a navmesh
                    //We do want to move if we hit an interactable and we want to begin the interaction, we also don't want to move once inside the interaction.
                    if (!isInInteraction)
                    {
                        if (NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, path))
                            GetComponent<NavMeshAgent>().SetDestination(hit.point);
                        if (!mouseInput && hit.transform.gameObject.tag == "Interactable")
                        {
                            //move to object, then bring up UI for interactions
                            GetComponent<NavMeshAgent>().SetDestination(hit.point);
                            hit.transform.GetComponent<Interactable>().OpenInteractable();
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
        }
	}
}

