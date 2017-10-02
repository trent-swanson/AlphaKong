using System;
using UnityEngine;

public class DriftCamera : MonoBehaviour
{
    [Serializable]
    public class AdvancedOptions
    {
        public bool updateCameraInUpdate;
        public bool updateCameraInFixedUpdate = true;
        public bool updateCameraInLateUpdate;
        //public KeyCode switchRightViewKey = KeyCode.RightArrow;
        public KeyCode switchCentreViewKey = KeyCode.Space;
    }

    public float smoothing = 6f;
    // The cameras position. The camera sits here.
    public Transform positionTarget;
    // The pivot the camera looks at and rotates around. This is so that the camera is always centered around the car.
    public Transform pivotTarget;
    // The point the camera looks at.
    // Technically, the camera object will LookAt() the pivotTarget which will then LookAt() the lookAtTarget().
    // Should switch between the frontOfCar and the centreOfMap.
    [HideInInspector]
    public Transform lookAtTarget;
    // The front of the car. This exists so that the camera can use this point as a default LookAt() target. 
    // By default the camera looks here.
    public Transform frontOfCar;
    // The centre of the map. The camera can be toggled to look at this.
    // This causes the pivotTarget to LookAt() the centre of the map.
    public Transform centreOfMap;
    //// If the player wants to look at the side of their car they can.
    //// The camera will shift its position and rotation to match this transform.
    //public Transform sideView;

    public AdvancedOptions advancedOptions;

    //bool m_ShowingRightSideView;
    bool showingCentreView = false;

    void Start()
    {
        // rotates the camera towards the pivot point.
        positionTarget.LookAt(pivotTarget);
    }

    private void FixedUpdate()
    {
        if (advancedOptions.updateCameraInFixedUpdate)
            UpdateCamera();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(advancedOptions.switchRightViewKey))
        //    m_ShowingRightSideView = !m_ShowingRightSideView;
        if (Input.GetKeyDown(advancedOptions.switchCentreViewKey))
            showingCentreView = !showingCentreView;

        if (advancedOptions.updateCameraInUpdate)
            UpdateCamera();
    }

    private void LateUpdate()
    {
        if (advancedOptions.updateCameraInLateUpdate)
            UpdateCamera();
    }

    private void UpdateCamera()
    {

        // Allows swapping between different camera views.
        // This if statement can be copied and new versions can be implemented to
        // provide the player with the ability to view their car from different angles
        // or view different objects and events in the map.
        if (showingCentreView)
        {
            //// Puts the camera next to the car, showing its side.
            //transform.position = Vector3.Lerp(transform.position, centreOfMap.position, Time.deltaTime * smoothing);
            //transform.rotation = Quaternion.Slerp(transform.rotation, centreOfMap.rotation, Time.deltaTime * smoothing);

            // puts the camera in the right place.
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Slerp(transform.rotation, positionTarget.rotation, Time.deltaTime * smoothing);

            // rotates pivotTarget to look at the centre of the map.
            pivotTarget.transform.LookAt(centreOfMap);
        }
        else
        {

            // puts the camera in the right place.
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Slerp(transform.rotation, positionTarget.rotation, Time.deltaTime * smoothing);

            // rotates pivotTarget to look at the front of the car.
            pivotTarget.transform.LookAt(frontOfCar);
        }
    }
}
