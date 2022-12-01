using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        ColorLastTarget = ColorGreen;
    }

    public Transform BlueTarget;
    public Transform GreenTarget;
    public string ColorBlue = "Blue"; 
    public string ColorGreen = "Green";
    public string ColorLastTarget;
    

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        BlueTarget.localPosition = new Vector3(Random.value * 8-4, 0.5f, Random.value * 8-4);
        GreenTarget.localPosition = new Vector3(Random.value * 8-4, 0.5f, Random.value * 8-4);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(GreenTarget.localPosition);
        sensor.AddObservation(BlueTarget.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);
        
        float distanceToGreen = Vector3.Distance(this.transform.localPosition, GreenTarget.localPosition);
        float distanceToBlue = Vector3.Distance(this.transform.localPosition, BlueTarget.localPosition);

        if(distanceToBlue < 1.42f && ColorLastTarget == ColorGreen)
        {
            SetReward(1.0f);
            EndEpisode();
            ColorLastTarget = ColorBlue;
        }
        else if(distanceToGreen < 1.42f && ColorLastTarget == ColorBlue)
        {
            SetReward(1.0f);
            EndEpisode();
            ColorLastTarget = ColorGreen;
        }
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }
}
