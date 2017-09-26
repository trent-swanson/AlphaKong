using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class CarController : MonoBehaviour
{
    public WheelCollider[] wheelColliders;
    public float enginePower = 150.0f;
    public float power = 0.0f;
    public float brake = 0.0f;
    public float steer = 0.0f;
    private float maxSteer = 15.0f;
    private float driftSteer = 35.0f;
    public bool driftbool = false;

    public XboxController controller;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f, -0.5f, 0.3f);
        for (int i = 0; i < wheelColliders.Length; ++i)
        {
            wheelColliders[i] = GameObject.FindGameObjectWithTag("Wheel").GetComponent<WheelCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(driftbool);
        power = XCI.GetAxis(XboxAxis.RightTrigger, controller) * enginePower * Time.deltaTime;
        brake = XCI.GetAxis(XboxAxis.LeftTrigger, controller) * 3000;
        steer = XCI.GetAxis(XboxAxis.LeftStickX, controller) * maxSteer;

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
        //brake
        if (brake > 0.0)
        {
            wheelColliders[0].brakeTorque = brake;
            wheelColliders[1].brakeTorque = brake;
            wheelColliders[2].brakeTorque = brake;
            wheelColliders[3].brakeTorque = brake;
            wheelColliders[0].motorTorque = 0.0f;
            wheelColliders[1].motorTorque = 0.0f;
            wheelColliders[2].motorTorque = 0.0f;
            wheelColliders[3].motorTorque = 0.0f;
        }
        else
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
    }
}
