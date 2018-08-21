using System.Collections.Generic;
using UnityEngine;

public class VehicleEntity : MonoBehaviour
{
    public List<AxleData> Axles;

    void Start()
    {
        foreach (AxleData axle in Axles)
        {
            axle.LeftWheelCollider.ConfigureVehicleSubsteps(5, 10, 12);
            axle.RightWheelCollider.ConfigureVehicleSubsteps(5, 10, 12);
        }
    }

    void Update()
    {
        var motor = 0;
        var steering = 0;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            motor = 400;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            motor = -400;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            steering = 45;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            steering = -45;
        }

        foreach (AxleData axle in Axles)
        {
            if (axle.Steering)
            {
                axle.LeftWheelCollider.steerAngle = steering;
                axle.RightWheelCollider.steerAngle = steering;
            }
            if (axle.Motor)
            {
                axle.LeftWheelCollider.motorTorque = motor;
                axle.RightWheelCollider.motorTorque = motor;
            }

            ApplyColliderParametersToWheel(axle.LeftWheel, axle.LeftWheelCollider);
            ApplyColliderParametersToWheel(axle.RightWheel, axle.RightWheelCollider);
        }
    }

    private void ApplyColliderParametersToWheel(GameObject wheel, WheelCollider collider)
    {
        Vector3 position;
        Quaternion quaternion;

        collider.GetWorldPose(out position, out quaternion);

        wheel.transform.position = position;
        wheel.transform.eulerAngles = quaternion.eulerAngles + new Vector3(90, 0, 90);
    }
}
