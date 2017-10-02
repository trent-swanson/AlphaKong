using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class CarController : MonoBehaviour
{
	public XboxController controller;

	[Space]

	public float enginePower = 100.0f;
	public float MaxFWVelocity = 10.0f;
    public float MaxBWVelocity = 5.0f;
	public float maxSteer = 15.0f;
	public float driftSteer = 35.0f;
	public float acclBrake = 3000.0f;
	public float declBrake = 3000.0f;
	public float playerDrag = 0.5f;
    public float speedMultiplyer = 0.0f;

	//[HideInInspector]
	public float power = 0.0f;
	[HideInInspector]
	public float reverse = 0.0f;
	[HideInInspector]
	public float steer = 0.0f;
	[HideInInspector]
	public bool driftbool = false;

    private Rigidbody playerBody;
	public Vector3 localVel;

	[Space]
	[Space]

	public WheelCollider[] wheelColliders;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f, -0.5f, 0.3f);
        playerBody = GetComponent<Rigidbody>();
    }

	private void Break(float breakValue)
    {
		wheelColliders[0].motorTorque = 0;
		wheelColliders[1].motorTorque = 0;
		wheelColliders[2].motorTorque = 0;
		wheelColliders[3].motorTorque = 0;
		wheelColliders[0].brakeTorque = breakValue;
		wheelColliders[1].brakeTorque = breakValue;
		wheelColliders[2].brakeTorque = breakValue;
		wheelColliders[3].brakeTorque = breakValue;
    }

    private void Reverse()
    {
		if (localVel.z > 0)
		{
			Debug.Log ("R Breaking");
			Break(declBrake * reverse);
			return;
		}

		Debug.Log ("Reverse");

		wheelColliders[0].brakeTorque = 0;
		wheelColliders[1].brakeTorque = 0;
		wheelColliders[2].brakeTorque = 0;
		wheelColliders[3].brakeTorque = 0;

        wheelColliders[0].motorTorque = 0;
        wheelColliders[1].motorTorque = -reverse;
        wheelColliders[2].motorTorque = -reverse;
        wheelColliders[3].motorTorque = 0;
    }

    private void Drift()
    {
        //Drift Steering
        if (XCI.GetButton(XboxButton.X))
        {
            driftbool = true;
            steer = 0;
            steer = -XCI.GetAxis(XboxAxis.LeftStickX, controller) * driftSteer;
            wheelColliders[1].steerAngle = steer;
            wheelColliders[2].steerAngle = steer;
            wheelColliders[0].steerAngle = 0;
            wheelColliders[3].steerAngle = 0;
        }

        else
        {
            driftbool = false;
            steer = 0;
            steer = XCI.GetAxis(XboxAxis.LeftStickX, controller) * maxSteer;
            wheelColliders[1].steerAngle = 0;
            wheelColliders[2].steerAngle = 0;
            wheelColliders[0].steerAngle = steer;
            wheelColliders[3].steerAngle = steer;
        }
    }

    private void Accelerate()
    {

		if (localVel.z < 0)
		{
			//Debug.Log ("A-Breaking");
			Break(acclBrake * power);
			return;
		}

        Debug.Log(wheelColliders[0].motorTorque);

        //Debug.Log ("Accelerate");
        wheelColliders[0].brakeTorque = 0;
        wheelColliders[1].brakeTorque = 0;
        wheelColliders[2].brakeTorque = 0;
        wheelColliders[3].brakeTorque = 0;

        //toggle drift mode
        //if (driftbool == true)
        //{
            //back 
            wheelColliders[1].motorTorque = power;
            wheelColliders[2].motorTorque = power;
            //front
            wheelColliders[0].motorTorque = 0;
            wheelColliders[3].motorTorque = 0;
        //}

        //else
        //{
        //    //front
        //    wheelColliders[0].motorTorque = power;
        //    wheelColliders[3].motorTorque = power;
        //    //back
        //    wheelColliders[1].motorTorque = 0;
        //    wheelColliders[2].motorTorque = 0;
        //}
    }


    // Update is called once per frame
    void Update()
    {
		power = XCI.GetAxis (XboxAxis.RightTrigger, controller) * (enginePower * speedMultiplyer) * Time.deltaTime;
		reverse = XCI.GetAxis (XboxAxis.LeftTrigger, controller) * (enginePower * speedMultiplyer) * Time.deltaTime;
		localVel = playerBody.transform.InverseTransformDirection(playerBody.velocity);

		RaycastHit hit;
		Ray groundCheck = new Ray(transform.position, Vector3.down);
		Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.red);
		if (Physics.Raycast(groundCheck, out hit, 0.5f))
		{
			if (hit.collider.tag == "Ground")
			{
				//playerBody.drag = playerDrag;
				//playerBody.angularDrag = playerDrag;
			}
		}

        if (localVel.z >= MaxFWVelocity)
        {
            float temp = localVel.z - MaxFWVelocity;
            localVel.z -= temp;
        }

        if (localVel.z <= -MaxBWVelocity)
        {
            float temp = localVel.z + MaxBWVelocity;
            localVel.z -= temp;
        }

        //Allows for drift mode
        Drift();

		if ((XCI.GetAxis (XboxAxis.LeftTrigger, controller) > 0) & (XCI.GetAxis (XboxAxis.RightTrigger, controller) > 0))
        {
			Break(acclBrake);
		}
		else if (XCI.GetAxis (XboxAxis.RightTrigger, controller) > 0)
        {
			Accelerate();
		}
		else if (XCI.GetAxis (XboxAxis.LeftTrigger, controller) > 0)
        {
			Reverse();
		}
        else
        {
            wheelColliders[0].motorTorque = 0;
            wheelColliders[1].motorTorque = 0;
            wheelColliders[2].motorTorque = 0;
            wheelColliders[3].motorTorque = 0;
        }
    }
}
