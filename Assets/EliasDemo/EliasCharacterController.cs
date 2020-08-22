using System.Collections;
using UnityEngine;

public class EliasCharacterController : MonoBehaviour
{
	public Transform myCamera;
	public float movementSpeed;
	public float rotationSpeed;

	private PlayerControls myControllerListener = null;

	private void Awake()
	{
		myControllerListener = PlayerControls.CreateWithKeyboardBindings();
	}

	private void Update()
	{
		UpdateRotation();
		UpdateMovement();
	}

	private void UpdateMovement()
	{
		float deltaSpeed = movementSpeed * Time.deltaTime;

        Vector2 leftStickAxis = myControllerListener.Movement;

		transform.position += transform.forward * (leftStickAxis.y * deltaSpeed);
	}

	private void UpdateRotation()
	{
		float deltaRotation = rotationSpeed * Time.deltaTime;

		Vector2 leftStickAxis = myControllerListener.Movement;
		float rotationDirection = leftStickAxis.x;

		transform.forward = Quaternion.AngleAxis(rotationDirection * deltaRotation, transform.up) * myCamera.forward;
		//Vector3 angle = myCamera.transform.eulerAngles;
		//if (angle.x > 50 && angle.x < 180)
		//{
		//	angle.x = 50;
		//}
		//else if (angle.x < 300 && angle.x > 180)
		//{
		//	angle.x = 300;	
		//}
		//myCamera.transform.eulerAngles = angle;
	}
}