using System.Collections.Generic;
using UdonToolkit;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class NyxCameraSystem : UdonSharpBehaviour
{
    [Header("General Settings")]
    [Tooltip("This UdonBehavior.")]
    public UdonBehaviour cameraController;
    [Tooltip("Preview material to be duplicated onto all the UI Buttons' render textures. (Probably an Unlit/Texture type).")]
    public Material previewMat;

    [Header("Main Camera Setup")]
    [Tooltip("The main camera whose output is used for recording/streaming, etc.")]
    public Camera mainCamera;
    [Tooltip("The MeshRenderer that shows the main camera preview.")]
    public MeshRenderer mainCamImage;
    [Tooltip("The main camera's render texture. - This can be used in any other Materials/systems as well to read what the camera is seeing.")]
    public RenderTexture mainCamTex;

    [Header("Preview Cameras")]
    [Tooltip("The preview camera for capturing different positions' views - recycled to check all points defined below.  Updated one position per frame.")]
    public Camera previewCamera;
    [ListView("CameraConfig")][LVHeader("RenderTex")]
    public RenderTexture[] camTex;
    [ListView("CameraConfig")][LVHeader("Position")]
    public Transform[] camPosition;
    [ListView("CameraConfig")][LVHeader("UI Button")]
    public MeshRenderer[] camButton;
    [ListView("CameraConfig")][LVHeader("AutoCycle")]
    public bool[] camAutoCycle;

    [Header("Lock Controls")]
    [UdonSynced]
    [Tooltip("Lock Camera Position by default.  The camera will not swap for any reason while locked. Send the 'lockToggle' event to change.")]
    public bool camLocked = false;
    private bool _localCamLocked;
    [Tooltip("The UI Toggle that handles toggling Lock mode.")]
    public Toggle camLockToggle;
    [Header("AutoCam Options")]
    [UdonSynced]
    [Tooltip("Automatically switch between cameras who have the AutoCycle option checked on an interval. Send the 'toggleAutoCam' event to change.")]
    public bool autoCycle = false;
    private bool _localAutoCycle;
    private int _localCurrentAutoCam = 0;
    private float _localDelayAutoCam = 0;
    private float _lastLocalAutoCamUpdate = 0;
    private int[] _autoEnabledCams;

    [Tooltip("If a view is clicked in AutoCycle mode, it will pause this many seconds before resuming cycling.")]
    public float delayOnOverride = 60f;
    private bool _manualOverride;
    private float _manualOverrideTime;
    [Tooltip("In Auto Cycle mode, Randomize the order that the cameras switch to instead of moving in sequence.")]
    public bool randomizeCamera = true;
    [Tooltip("Minimum seconds to wait before auto-cycling camera.")]
    public float autoUpdateSecondsLow = 10;
    [Tooltip("Maximum seconds to wait before auto-cycling camera.")]
    public float autoUpdateSecondsHigh = 20;
    [Tooltip("The UI Toggle that handles toggling Lock mode.")]
    public Toggle autoCycleToggle;

    // Positional LERPing
    [Header("Extra Settings")]
    [UdonSynced]
    [Tooltip("LERP between points when changing cameras for a smoother transition (fly-to).")]
    public bool lerpPosition = true;
    [Tooltip("UI Toggle that turns lerping behavior on and off.")]
    public Toggle lerpToggle;
    [Tooltip("Transition time between positions (seconds)")]
    public float lerpTime = 0.5f;
    private float _lerpSpeed;
    private float _lerpDistanceTotal;
    private float _lerpDistanceTraveled;
    private float _lerpStartTime;
    private bool _lerping;
    private Transform _lerpStartPosition;

    // internals for syncing preview camera position
    private GameObject[] _previewCameras;
    private int _camCount;
    private int _curCam;
    // and the live camera position
    [UdonSynced]
    private int _activeCam = 0;
    private int _localActiveCam = 5318008;

    [Header("Debugging")]
    [Tooltip("Log to debug log. Tag: (EventCameraSystem)")]
    public bool logDebug;
    private void logStuff(string message){
        if (logDebug){
            Debug.Log("[Eremite](EventCameraSystem) " + message);
        }
    }

    void Start(){
        logStuff("Beginning intitial setup.");
        _camCount = camPosition.Length;
        _curCam = 0;

        logStuff("Setting up camera materials and render textures.");
        mainCamImage.material = previewMat;
        mainCamImage.material.mainTexture = mainCamTex;
        logStuff(" * Finished populating Main Camera.");
        for (int i=0 ; i<_camCount ; i++) {
            camButton[i].material = previewMat;
            camButton[i].material.mainTexture = camTex[i];
            logStuff(" * Finished populating preview camera # " + i.ToString());
        }
        autoCycleToggle.isOn = autoCycle;
        camLockToggle.isOn = camLocked;
        lerpToggle.isOn = lerpPosition;
        // Get a list of cameras that have auto-cycle enabled. Gotta be a better way to do this.
        logStuff("Populating array of cameras with Auto Cycling enabled.");
        int _autoCount = 0;
        for ( int i = 0 ; i < camAutoCycle.Length ; i++ ) {
            if (camAutoCycle[i]) {
                _autoCount+=1;
            }
        }
        _autoEnabledCams = new int[_autoCount];
        logStuff("Total Cameras in AutoCycle AllowList: " + _autoCount.ToString());
        _autoCount = 0;
        for ( int i = 0 ; i < camAutoCycle.Length ; i++ ) {
            if (camAutoCycle[i]) {
                _autoEnabledCams[_autoCount] = i;
                logStuff(" * Added preview cam " + i.ToString() + " to AutoAllow (ArrayPos=" + _autoCount + ")");
                _autoCount += 1;
            }
        }
        logStuff("End of Startup.");
    }

    // Required for setting UdonSynced vars. 
    public void TakeOwner(){
        if (!Networking.IsOwner(cameraController.gameObject)) {
            Networking.SetOwner(Networking.LocalPlayer, cameraController.gameObject);
        }
    }

    void LateUpdate(){
        // Move the preview cam to each configured position (one per frame) and update its RenderTex.
        previewCamera.transform.position = camPosition[_curCam].position;
        previewCamera.transform.rotation = camPosition[_curCam].rotation;
        previewCamera.targetTexture = camTex[_curCam];
        previewCamera.enabled = true;
        previewCamera.Render();
        previewCamera.enabled = false;
        if ( _curCam < _camCount - 1 ) {
            _curCam += 1;
        } else {
            _curCam = 0;
        }
        if (_localAutoCycle && !_localCamLocked) {
            if ( _manualOverride == true ) {
                if  (Time.unscaledTime - _manualOverrideTime > delayOnOverride){
                    _manualOverride = false;
                    logStuff("Manual Override timeout elapsed - Resuming Auto Cycle.");
                }
            }
            if ( Time.unscaledTime - _lastLocalAutoCamUpdate > _localDelayAutoCam && _manualOverride == false) {
                _lastLocalAutoCamUpdate = Time.unscaledTime;
                _localDelayAutoCam = fakeRandom(_lastLocalAutoCamUpdate, autoUpdateSecondsLow, autoUpdateSecondsHigh, false);
                TakeOwner();
                if (randomizeCamera) {
                    _localCurrentAutoCam = (int)fakeRandom(_lastLocalAutoCamUpdate, 0.0f, (float)(_autoEnabledCams.Length - 1), true);
                    logStuff("AutoCam random switch to camera: " + _autoEnabledCams[_localCurrentAutoCam].ToString());
                } else {
                    if (_localCurrentAutoCam < _autoEnabledCams.Length - 1 ) {
                        _localCurrentAutoCam += 1;
                    } else {
                        _localCurrentAutoCam = 0;
                    }
                    logStuff("AutoCam moving to next allowed camera: " + _autoEnabledCams[_localCurrentAutoCam].ToString());
                }
                _activeCam = _autoEnabledCams[_localCurrentAutoCam];
                OnDeserialization();
                RequestSerialization();
            }
        }
        if (lerpPosition && _lerping){
            float _traveled = (Time.unscaledTime - _lerpStartTime) * _lerpSpeed;
            float percentTraveled = _traveled / _lerpDistanceTotal;
            mainCamera.transform.position = Vector3.Lerp(_lerpStartPosition.position, camPosition[_localActiveCam].position, percentTraveled);
            mainCamera.transform.rotation = Quaternion.Lerp(_lerpStartPosition.rotation, camPosition[_localActiveCam].rotation, percentTraveled);
            if (percentTraveled >= 1.0f) {
                _lerping = false;
                logStuff("Camera has LERP'd its way to its new position.");
            }
        }
    }

    public override void OnDeserialization()
    {
        if ( _activeCam != _localActiveCam ) {
            if (lerpPosition) {
                _lerpStartPosition = mainCamera.transform;
                _lerpStartTime = Time.unscaledTime;
                _lerpDistanceTotal = Vector3.Distance(_lerpStartPosition.position, camPosition[_activeCam].position);
                _lerpSpeed = _lerpDistanceTotal / lerpTime;
                _lerping = true;
                logStuff("Lerping to new cam position.");
            }
            _localActiveCam = _activeCam;
            if (!lerpPosition){
                mainCamera.transform.position = camPosition[_localActiveCam].position;
                mainCamera.transform.rotation = camPosition[_localActiveCam].rotation;
                logStuff("Moved instantly to new cam position.");
            }
        }
        if (_localAutoCycle != autoCycle) {
            _localAutoCycle = autoCycle;
            autoCycleToggle.isOn = _localAutoCycle;
            logStuff("Toggled AutoCycle to " + _localAutoCycle.ToString());
        }
        if (_localCamLocked != camLocked) {
            _localCamLocked = camLocked;
            camLockToggle.isOn = _localCamLocked;
            logStuff("Toggled Cam Locking to " + _localCamLocked.ToString());
        }
    }

    public void updateMainCam(){
        for ( int i=0 ; i < _camCount ; i++ ) {
            if ( camButton[i].enabled == false ){
                if (!camLocked) {
                    TakeOwner();
                    _activeCam = i;
                }
                camButton[i].enabled = true;
                break;
            }
        }
        if (autoCycle) {
            _manualOverride = true;
            _manualOverrideTime = Time.unscaledTime;
        }
        RequestSerialization();
        OnDeserialization();
    }

    public void lockToggle() {
        TakeOwner();
        camLocked = camLockToggle.isOn;
        RequestSerialization();
        OnDeserialization();
    }

    public void toggleAutoCam(){
        TakeOwner();
        autoCycle = autoCycleToggle.isOn;
        RequestSerialization();
        OnDeserialization();
    }

    public void toggleLerpPosition(){
        TakeOwner();
        lerpPosition = lerpToggle.isOn;
        RequestSerialization();
        OnDeserialization();
    }

    // I hate this, but it's somehow better than having to Re Init Unity's Random every time.
    private float fakeRandom(float seed, float min, float max, bool introunding) {
        // Modulo (Max-Min) gives a number between 0 and (Max-Min) ; then adding Min gives us between Min and Max
        // To account for rounding, min and max are adjusted so min/max values aren't half as likely as the other possibilities.
        // (max+.5)-(min-.5) == max + .5 - min + .5 == max - min + 1 
        float fake;
        fake = ((seed*12345) % (max-min + (introunding ? 1 : 0 ))) + min; 
        logStuff("Random between " + min.ToString() + " and " + max.ToString() + " : " + fake.ToString() + " | Int: " + ((int)fake).ToString());
        return fake;
    }
}