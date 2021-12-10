using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveTo : Agent
{
    Rigidbody rBody;
    float timer;
    Vector3 posIni;
    void Start () {
        rBody = GetComponent<Rigidbody>();
        posIni = this.transform.localPosition;
    }

    public Transform Target;
    void Update(){

    }
    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);


        // Fell off platform
        if (this.transform.localPosition.y < 0 )
        {
        this.transform.localPosition = posIni;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
            EndEpisode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        // if (collision.gameObject.tag == "wall")
        // {
        // this.transform.localPosition = posIni;
        // this.rBody.angularVelocity = Vector3.zero;
        // this.rBody.velocity = Vector3.zero;
        //     EndEpisode();
        // }
        if (collision.gameObject.tag == "agent")
        {
            SetReward(-0.5f);
                    this.transform.localPosition = posIni;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
            EndEpisode();
        }
        if (collision.gameObject.tag == "Target")
        {
            SetReward(1.0f);
                    Target.localPosition = new Vector3(Random.value * 3 - 3,
                                           0.5f,
                                           Random.value * 3 - 3);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}