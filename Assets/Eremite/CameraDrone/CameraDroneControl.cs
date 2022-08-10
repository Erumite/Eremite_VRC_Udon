
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CameraDroneControl : UdonSharpBehaviour
{
    [Header("Drone Internals")]
    [Tooltip("Game Object of the drone for taking ownership.")]
    public GameObject drone;
    [Tooltip("The drone's camera.")]
    public Camera droneCamera;
    [Tooltip("Constant Force script on the drone")]
    public ConstantForce droneForce;
    

    [Header("Debugging")]
    public bool debugLog = false;

    private void logStuff(string message){
        if (debugLog){
            Debug.Log("[Eremite](CameraDrone) " + message);
        }
    }

    void Start()
    {
        // nothin, lol
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, drone);
        droneCamera.enabled = true;
    }

    public override void OnDrop()
    {
        droneCamera.enabled = false;
    }

    public override void OnPickupUseDown(){
        logStuff("Drone Deactivated - Disabling force.");
        droneForce.enabled = true;
    }

    public override void OnPickupUseUp()
    {
        logStuff("Drone activated - Enabling force.");
        droneForce.enabled = false;
    }
}
