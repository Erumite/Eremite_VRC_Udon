
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class SuperSimplePen : UdonSharpBehaviour
{
    public TrailRenderer inkTrailRenderer;
    private bool writing = false;
    private bool localWriting = false;

    void Start()
    {
        inkTrailRenderer.emitting = false;
    }

    public override void OnDeserialization()
    {
        if ( writing != localWriting ) { 
            localWriting = writing;
            inkTrailRenderer.emitting = localWriting;
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        writing = true;
        RequestSerialization();
        OnDeserialization();
    }

    public override void OnPickupUseUp()
    {
        writing = false;
        RequestSerialization();
        OnDeserialization();
    }
}
