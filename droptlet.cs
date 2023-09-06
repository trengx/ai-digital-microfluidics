//https://unity-technologies.github.io/ml-agents/Learning-Environment-Create-New/
// PS C:\Users\yshin> cd venv
// PS C:\Users\yshin\venv> .\scripts\ctivate
// (venv) PS C:\Users\yshin\venv> mlagents-learn config.yaml --run-id=RollerBall6
// (venv2) C:\Users\jun\unity\venv2>tensorboard --logdir results --port 6006

using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Droplet : Agent
{
    Rigidbody rBody;
    void Start () {
        rBody = GetComponent<Rigidbody>();
    }

    public CameraSensorComponent cameraSensor;
    public Transform Target;
    public Transform Obstacle1;
    //public Transform Obstacle2;
    //public Transform Obstacle3;
    public override void OnEpisodeBegin()
    {

        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3( -3, 0.05f, -3);
    

        // Move the target to a new spot
        // Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           //0.00005f,
                                           //Random.value * 8 - 4);

        Obstacle1.localPosition = new Vector3( 0, 0.05f, 0);
        //Obstacle2.localPosition = new Vector3( 3, 0.05f, -3);
        //Obstacle3.localPosition = new Vector3( -2.5f, 0.05f, -1.5f);
        // Calculate the midpoint between the Droplet and the Target
        // Vector3 midpoint = (this.transform.localPosition + Target.localPosition) / 2;

        // Set the position of the Obstacle to the calculated midpoint
        // Keeping the y-coordinate same as before for the obstacle
        // Obstacle1.localPosition = new Vector3(midpoint.x, 0.05f, midpoint.z);

    }




    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        // Debug.Log("Heuristic Input: " + continuousActionsOut[0] + ", " + continuousActionsOut[1]);
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        // The observations from the camera will be automatically collected 
        // due to the CameraSensorComponent attached to the agent.
        // So, you don't need to manually add observations here.

        /*
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        sensor.AddObservation(Obstacle1.localPosition);
        //sensor.AddObservation(Obstacle2.localPosition);
        */
    }

    public float forceMultiplier = 0.1f;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Give a small negative reward every step to encourage faster completion
        SetReward(-0.01f);
        // SetReward(0.1f/distanceToTarget);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            Debug.Log("Target reached");
            SetReward(1.0f);
            EndEpisode();
        }


        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            Debug.Log("Fell off");
            SetReward(-1.0f);
            EndEpisode();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle1"))
        {
            Debug.Log("Obstacle collided");
            SetReward(-0.5f);
            EndEpisode();
        }
    }

}
