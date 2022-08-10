using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StreamStation : UdonSharpBehaviour
{
    [Header("Objects - Mouseover for Detail")]
    [Tooltip("Game object with udon behavior on it.")]
    public GameObject stationScriptGameObject;
    [Tooltip("Cameras that should be toggled off until station is entered - enabled while seated.")]
    public Camera[] streamCameras;
    [Tooltip("Blocker Colliders are colliders that spawn around the streamer once seated to prevent other players from walking near them.")]
    public GameObject[] blockerColliders;
    [Tooltip("Occlusion Blocks are objects meant to cause the streamer to occlude anything not seen through the camera to save frames.")]
    public OcclusionPortal streamerOcclusionPortal;
    [Tooltip("Post Processing game objects that should be disabled while seated.")]
    public GameObject[] postProcessing;
    [Tooltip("The mesh for the mesh with the overrender texture: On while seated to override screenspace with camera output.")]
    public MeshRenderer streamShaderCapsule;
    [Tooltip("Streamer shader capsule material to use. (in case of multiple streams/viewpoints needed, this can be swapped in Editor)")]
    public Material streamCapsuleMaterial;
    [Tooltip("The mesh for the chair is disabled while seated, enabled while not seated.")]
    public MeshRenderer chairMeshRenderer;
    [Tooltip("Label for the chair, if applicable: Useful for differentiating between chairs.")]
    public MeshRenderer labelTextMesh;
    [UdonSynced]
    private bool isSeated;
    private bool isSeatedLocal;

    [Header("Debugging")]
    [Tooltip("Enable debug logging - Tag: (StreamStation)")]
    public bool debugLog = false;
    private void logStuff(string message){
        if (debugLog){
            Debug.Log("[Eremite](StreamStation) " + message);
        }
    }

    async void Start()
    {
        // Initialize the default state.
        streamerOcclusionPortal.open = true;
        streamShaderCapsule.material = streamCapsuleMaterial;
        streamShaderCapsule.enabled = false;
        labelTextMesh.enabled = true;
        chairMeshRenderer.enabled = true;
        for (int i=0; i < blockerColliders.Length ; i++){
            blockerColliders[i].SetActive(false);
        }
        for (int i=0; i < streamCameras.Length ; i++ ) {
            streamCameras[i].enabled = false;
        }
    }
    void OnStationEntered(VRCPlayerApi player){
        if (player.isLocal){
            Networking.SetOwner(player, stationScriptGameObject);
            streamShaderCapsule.enabled = true;
            streamerOcclusionPortal.open = false;
            for (int i=0; i < postProcessing.Length; i++) {
                postProcessing[i].SetActive(false);
            }
            for (int i=0; i < streamCameras.Length ; i++ ) {
                streamCameras[i].enabled = true;
            }
            isSeated = true;
            logStuff("You have entered the streamer station. (local)");
        }
        logStuff(player.displayName + " : Entered Station.  isSeated: " + isSeated.ToString());
        RequestSerialization();
        OnDeserialization();
    }
    void OnStationExited(VRCPlayerApi player){
        if (player.isLocal){
            streamShaderCapsule.enabled = false;
            streamerOcclusionPortal.open = true;
            for (int i=0; i < postProcessing.Length; i++) {
                postProcessing[i].SetActive(true);
            }
            for (int i=0; i < streamCameras.Length ; i++ ) {
                streamCameras[i].enabled = false;
            }
            isSeated = false;
            logStuff("You have exited the streamer station. (local)");
        }
        logStuff(player.displayName + " : Exited Station.  isSeated: " + isSeated.ToString());
        RequestSerialization();
        OnDeserialization();
    }
    void Interact(){
        Networking.LocalPlayer.UseAttachedStation();
    }
    void OnDeserialization(){
        if (isSeated != isSeatedLocal) {
            isSeatedLocal = isSeated;
            syncChairState();
            logStuff("Updated chair state.  Seated: " + isSeated.ToString());
        }
    }
    private void syncChairState() {
        logStuff("Updating chair state.  Seated: " + isSeated.ToString());
        for (int i=0; i < blockerColliders.Length ; i++){
            blockerColliders[i].SetActive(isSeatedLocal);
        }
        labelTextMesh.enabled = !isSeatedLocal;
        chairMeshRenderer.enabled = !isSeatedLocal;
    }
}