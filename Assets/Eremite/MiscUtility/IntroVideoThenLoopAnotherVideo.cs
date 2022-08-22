
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;

public class IntroVideoThenLoopAnotherVideo : UdonSharpBehaviour
{
    [Tooltip("This video will play once on world load.")]
    public VRCUrl introVideo;
    [Tooltip("This video will loop after the intro vid has finished.")]
    public VRCUrl loopedVideo;
    [Tooltip("VRC Unity Video Player - Must be on the same object as this script.")]
    public VRCUnityVideoPlayer videoPlayer;
    private bool _introPlayed = false;

    void Start() {
        videoPlayer.Loop = false;
        videoPlayer.PlayURL(introVideo);
    }

    public override void OnVideoEnd()
    {
        Debug.Log("OnVideoEnd Called");
        if ( ! _introPlayed ) {
            videoPlayer.Stop(); 
            videoPlayer.Loop = true;
            videoPlayer.PlayURL(loopedVideo);
            videoPlayer.Play();
        }
    }
}
