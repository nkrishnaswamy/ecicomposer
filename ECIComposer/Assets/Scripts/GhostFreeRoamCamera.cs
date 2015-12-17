using UnityEngine;

using Helper;

[RequireComponent(typeof(Camera))]
public class GhostFreeRoamCamera : MonoBehaviour
{
	public float initialSpeed = 10f;
	public float increaseSpeed = 1.25f;
	
	public bool allowMovement = true;
	public bool allowRotation = true;

	/*
	public KeyCode forwardButton = KeyCode.W;
	public KeyCode backwardButton = KeyCode.S;
	public KeyCode rightButton = KeyCode.D;
	public KeyCode leftButton = KeyCode.A;
	*/

	public KeyCode resetButton = KeyCode.R;
	
	public float cursorSensitivity = 0.025f;
	public bool cursorToggleAllowed = true;
	public KeyCode cursorToggleButton = KeyCode.Escape;

	Inspector inspector;
	ObjectList objectList;
	ParameterList parameterList;


	Vector3 startPosition = Vector3.zero;
	Vector3 startRotation = Vector3.zero;
	
	float currentSpeed = 0f;
	bool moving = false;
	bool togglePressed = false;


	void Start()
	{
		inspector = GameObject.Find ("Inspector").GetComponent ("Inspector") as Inspector;
		objectList = GameObject.Find ("ObjectList").GetComponent ("ObjectList") as ObjectList;
		parameterList = GameObject.Find ("ParameterList").GetComponent ("ParameterList") as ParameterList;
		startPosition = transform.position;
		startRotation = transform.eulerAngles;
	}

	void OnEnable()
	{
		if (cursorToggleAllowed)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
	
	void Update()
	{
		if (allowMovement)
		{	
			
			bool lastMoving = moving;
			Vector3 deltaPosition = Vector3.zero;
			
			if (moving)
				currentSpeed += increaseSpeed * Time.deltaTime;
			
			moving = false;

			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				CheckMove(ref deltaPosition, transform.forward);
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				CheckMove(ref deltaPosition, -transform.forward);
			}

			if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
				if (Input.GetAxis("Mouse X")<0) { //left
					CheckMove(ref deltaPosition, transform.right);
				}
				if (Input.GetAxis("Mouse X")>0) { //right
					CheckMove(ref deltaPosition, -transform.right);
				}
				if (Input.GetAxis("Mouse Y")<0) { //backward
					CheckMove(ref deltaPosition, transform.up);
				}
				if (Input.GetAxis("Mouse Y")>0) { //forward
					CheckMove(ref deltaPosition, -transform.up);
				}
			}

			if (Input.GetKey(KeyCode.R)) {
				transform.position = startPosition;
				transform.eulerAngles = startRotation;
			}

			/*
			//WASD envoking methods 
			CheckMove(forwardButton, ref deltaPosition, transform.forward);
			CheckMove(backwardButton, ref deltaPosition, -transform.forward);
			CheckMove(rightButton, ref deltaPosition, transform.right);
			CheckMove(leftButton, ref deltaPosition, -transform.right);
			*/

			if (moving)
			{
				if (moving != lastMoving)
					currentSpeed = initialSpeed;
				
				transform.position += deltaPosition * currentSpeed * Time.deltaTime;
			}
			else currentSpeed = 0f;            
		}



		if (allowRotation)
		{	
			
			if (((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(0) && 
			    Helper.Helper.PointOutsideMaskedAreas(new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y),
			                                      new Rect[]{inspector.InspectorRect,objectList.bgRect,parameterList.bgRect})))
			{
				//Vector3 eulerAngles = transform.eulerAngles;
				//eulerAngles.x += -Input.Rotate("Mouse Y") * 359f * cursorSensitivity;
				//eulerAngles.y += Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
				//transform.eulerAngles = eulerAngles;
				if (Input.GetAxis("Mouse X")<0) { //left
					transform.RotateAround (inspector.transform.position, Vector3.down, 100 * Time.deltaTime);
				}
				if (Input.GetAxis("Mouse X")>0) { //right
					transform.RotateAround (inspector.transform.position, Vector3.up, 100 * Time.deltaTime);
				}
				if (Input.GetAxis("Mouse Y")<0) { //backward
					transform.RotateAround (inspector.transform.position, Vector3.right, 100 * Time.deltaTime);
				}
				if (Input.GetAxis("Mouse Y")>0) { //forward
					transform.RotateAround (inspector.transform.position, Vector3.left, 100 * Time.deltaTime);
				}
			}

		
		}
		
		if (cursorToggleAllowed)
		{
			if (Input.GetKey(cursorToggleButton))
			{
				if (!togglePressed)
				{
					togglePressed = true;
					Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
					Cursor.visible = !Cursor.visible;
				}
			}
			else togglePressed = false;
		}
		else
		{
			togglePressed = false;
			Cursor.visible = false;
		}
	}

	/*
	void CheckMove(KeyCode keyCode, ref Vector3 deltaPosition, Vector3 directionVector)
	{
		if (Input.GetKey(keyCode))
		{
			moving = true;
			deltaPosition += directionVector;
		}
	}
	*/

	//overloaded CheckMove method 
	void CheckMove(ref Vector3 deltaPosition, Vector3 directionVector)
	{

		moving = true;
		deltaPosition += directionVector;
	
	}

}
