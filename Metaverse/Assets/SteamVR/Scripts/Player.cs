using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float speed = 4.0f;
	public float shiftSpeed = 16.0f;
	public bool showInstructions = true;

	private Vector3 startEulerAngles;
	private Vector3 startMousePosition;
	private float realTime;
	GameObject player;
	GameObject direction;

	// Start is called before the first frame update
	void Start()
	{
	}

	void OnEnable()
	{
		realTime = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	void Update()
	{
		float forward = 0.0f;
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			forward += 1.0f;
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			forward -= 1.0f;
		}
		float up = 0.0f;
		if (Input.GetKey(KeyCode.E))
		{
			up += 1.0f;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			up -= 1.0f;
		}
		float right = 0.0f;
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			right += 1.0f;
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			right -= 1.0f;
		}
		float currentSpeed = speed;
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			currentSpeed = shiftSpeed;
		}
		float dif = 0;
		float realTimeNow = Time.realtimeSinceStartup;
		float deltaRealTime = realTimeNow - realTime;
		realTime = realTimeNow;
		Vector3 delta = new Vector3(right, up, forward) * currentSpeed * deltaRealTime;
		transform.position += transform.TransformDirection(delta);
		Vector3 alpha = new Vector3(
			transform.position.x,
			transform.position.y + 1.6f,
			transform.position.z
		);
		RaycastHit[] hitInfo = Physics.RaycastAll(alpha, Vector3.down, 10);
		if (hitInfo.Length > 0)
		{
			Vector3 point = new Vector3(0, 0, 0);
			for (int i = 0; i < hitInfo.Length; i++)
			{
				if (
					hitInfo[i].collider.gameObject.name != "HeadCollider"
					&& hitInfo[i].collider.gameObject.name != "LegCollider"
					&& (hitInfo[i].distance < dif || dif == 0))
				{
					dif = hitInfo[i].distance;
					point = hitInfo[i].point;
				}
			}
			// direction.transform.position = point + new Vector3(0.25f, 0, 0);
			transform.position = new Vector3(point.x, point.y + 1.7f, point.z);
		}

		Vector3 mousePosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(1) /* right mouse */)
		{
			startMousePosition = mousePosition;
			startEulerAngles = transform.localEulerAngles;
		}
		if (Input.GetMouseButton(1) /* right mouse */)
		{
			Vector3 offset = mousePosition - startMousePosition;
			Vector3 euler = startEulerAngles + new Vector3(-offset.y * 360.0f / Screen.height, offset.x * 360.0f / Screen.width, 0.0f);
			transform.localEulerAngles = euler;
		}
	}
}