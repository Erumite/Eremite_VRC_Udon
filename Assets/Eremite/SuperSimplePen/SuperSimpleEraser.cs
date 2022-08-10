
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SuperSimpleEraser : UdonSharpBehaviour
{
    public TrailRenderer inkTrailRenderer;
    void Start(){}
    
    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "eraseAll");
    }
    public void eraseAll(){
        inkTrailRenderer.time = 0;
        SendCustomEventDelayedFrames("reWrite", 1, VRC.Udon.Common.Enums.EventTiming.LateUpdate);
    }
    public void reWrite(){
        inkTrailRenderer.time = 99999;
    }
}
