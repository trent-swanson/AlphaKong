using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class CarController : MonoBehaviour
{
    public WheelCollider[] wheelColliders;
    private float enginePower = 1000.0f;
    public float power = 0.0f;
    private float MaxVelocity = 10.0f;
    public float reverse = 0.0f;
    public float brake = 0.0f;
    public float steer = 0.0f;
    private float maxSteer = 15.0f;
    private float driftSteer = 35.0f;
    private Rigidbody playerBody;
    public bool driftbool = false;
    public XboxController controller;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f, -0.5f, 0.3f);
        playerBody = GetComponent<Rigidbody>();

    }

    private void Break()
    {
        wheelColliders[0].brakeTorque = brake;
        wheelColliders[1].brakeTorque = brake;
        wheelColliders[2].brakeTorque = brake;
        wheelColliders[3].brakeTorque = brake;
    }

    private void Reverse()
    {
        wheelColliders[0].motorTorque = -reverse * 10;
        wheelColliders[1].motorTorque = -reverse * 10;
        wheelColliders[2].motorTorque = -reverse * 10;
        wheelColliders[3].motorTorque = -reverse * 10;
        wheelColliders[0].brakeTorque = 0;
        wheelColliders[1].brakeTorque = 0;
        wheelColliders[2].brakeTorque = 0;
        wheelColliders[3].brakeTorque = 0;
    }

    private void Drift()
    {
        //Drift Steering
        if (XCI.GetButton(XboxButton.X))
        {
            driftbool = true;
            steer = 0;
            steer = -XCI.GetAxis(XboxAxis.LeftStickX, controller) * driftSteer;
            wheelColliders[0].steerAngle = 0;
            wheelColliders[3].steerAngle = 0;
            wheelColliders[1].steerAngle = steer;
            wheelColliders[2].steerAngle = steer;
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
        wheelColliders[0].brakeTorque = 0;
        wheelColliders[1].brakeTorque = 0;
        wheelColliders[2].brakeTorque = 0;
        wheelColliders[3].brakeTorque = 0;
        //toggle drift mode
        if (driftbool == true)
        {
            //back 
            wheelColliders[1].motorTorque = power * 10;
            wheelColliders[2].motorTorque = power * 10;
            //front
            wheelColliders[0].motorTorque = 0;
            wheelColliders[3].motorTorque = 0;
        }
        else
        {
            //front
            wheelColliders[0].motorTorque = power * 10;
            wheelColliders[3].motorTorque = power * 10;
            //back
            wheelColliders[1].motorTorque = 0;
            wheelColliders[2].motorTorque = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        power = XCI.GetAxis(XboxAxis.RightTrigger, controller) * enginePower * Time.deltaTime;
        reverse = XCI.GetAxis(XboxAxis.LeftTrigger, controller) * enginePower * Time.deltaTime;
        brake = XCI.GetAxis(XboxAxis.LeftTrigger, controller) * 3000;
        Vector3 localVel = playerBody.transform.InverseTransformDirection(playerBody.velocity);

        //Allows for drift mode
        Drift();

        if (XCI.GetAxis(XboxAxis.RightTrigger, controller) > 0)
        {
            if (localVel.z >= 0)
            {
                Break();
            }
            if (playerBody.velocity.magnitude < MaxVelocity)
            {
                Accelerate();
                return;
            }
        }

        if (XCI.GetAxis(XboxAxis.LeftTrigger, controller) > 0)
        {
            Break();
            if (localVel.z <= 0)
            {
                Reverse();
            }
        }
    }
}
