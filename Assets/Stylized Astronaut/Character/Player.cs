using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

	public MazeGenerator Generator;
	
	private Animator anim;
	private CharacterController controller;

	public float speed = 600.0f;
	public float turnSpeed = 400.0f;
	private Vector3 moveDirection = Vector3.zero;
	public float gravity = 20.0f;

	void Start () {
		controller = GetComponent <CharacterController>();
		anim = gameObject.GetComponentInChildren<Animator>();
	}

	void Update (){
		if (Input.GetKey ("w")) {
			anim.SetInteger ("AnimationPar", 1);
		}  else {
			anim.SetInteger ("AnimationPar", 0);
		}

		if(controller.isGrounded){
			moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
		}

		float turn = Input.GetAxis("Horizontal");
		transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
		controller.Move(moveDirection * Time.deltaTime);
		moveDirection.y -= gravity * Time.deltaTime;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Unit")
		{
			int index = other.GetComponent<LoadNeighbours>().index;
			
			for (int i = -5; i < 5; i++)
			{
				if(Generator._units.TryGetValue(index + i, out Unit unit))
					unit.GameObject.SetActive(true);
			}
			
			for (int i = 0; i < 5; i++)
			{
				Debug.Log((index + i) * Generator.gridSize);
				if(Generator._units.TryGetValue((index + i) * Generator.gridSize, out Unit unit))
					unit.GameObject.SetActive(true);
			}
			for (int i = -6; i < -1; i++)
			{
				if(Generator._units.TryGetValue(index - i - Generator.gridSize, out Unit unit))
					unit.GameObject.SetActive(true);
			}
		}
	}
	
}
