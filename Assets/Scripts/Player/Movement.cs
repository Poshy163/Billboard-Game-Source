using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public float mouseSensitivityX = 1.0f;
	public float mouseSensitivityY = 1.0f;
	public float walkSpeed = 10.0f;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	Transform cameraT;
	float verticalLookRotation;
	Rigidbody rigidbodyR;
	float jumpForce = 250.0f;
	bool grounded;
	public LayerMask groundedMask;
	void Start ()
	{
		cameraT = Camera.main.transform;
		rigidbodyR = GetComponent<Rigidbody>();
		LockMouse();
	}
	void Update ()
	{
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		cameraT.localEulerAngles = Vector3.left * verticalLookRotation;
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical")).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);
		if(Input.GetButtonDown("Jump"))
		{
			if(grounded)
			{
				rigidbodyR.AddForce(transform.up * jumpForce);
			}
		}
		Ray ray = new Ray(transform.position,-transform.up);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit,1 + .1f,groundedMask))
		{
			grounded = true;
		}
		else
		{
			grounded = false;
		}

		/*  
		if(Input.GetMouseButtonUp(0))
		{
			if(!cursorVisible)
			{
				UnlockMouse();
			}
			else
			{
				LockMouse();
			}
		}*/
	}

	void FixedUpdate ()
	{
		rigidbodyR.MovePosition(rigidbodyR.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	void UnlockMouse ()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void LockMouse ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
