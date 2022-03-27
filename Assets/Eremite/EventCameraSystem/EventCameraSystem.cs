using System.Collections.Generic;
using UdonToolkit;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EventCameraSystem : UdonSharpBehaviour
{
    [Header("General Settings")]
    [Tooltip("This UdonBehavior.")]
    public UdonBehaviour cameraController;
    [Tooltip("Preview material to be duplicated onto all the UI Buttons' render textures. (Probably an Unlit/Texture type).")]
    public Material previewMat;
    [Tooltip("UI Text that displays the Owner's name.")]
    public Text ownerText;

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
    [Tooltip("Slider that shows time till next auto-cam.")]
    public Slider autoCyclePreviewSlider;

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
    private bool _localLerping;
    private Transform _lerpStartPosition;

    [Header("Special Effects")]
    [Tooltip("Mesh renderer for the quad that handles screen effects/overlays.\nMaterial: Will be applied to the mesh.\nAnim Trigger: Will be sent as an animation trigger to the FX plane.\nButton: The button that handles the preview/toggle.")]
    public MeshRenderer effectsQuad;
    [ListView("Effects")][LVHeader("Material")]
    public Material[] effectsMaterial;
    [ListView("Effects")][LVHeader("Anim Trigger")]
    public string[] effectsAnimTrigger;
    [ListView("Effects")][LVHeader("Button")]
    public Button[] effectsButton;
    [UdonSynced]
    private int _currentEffect;
    private int _localCurrentEffect;

    [Header("Look-At Settings")]
    [Tooltip("UI Dropdown that handles the optional LookAt targets.")]
    public Dropdown lookatDropdown;
    [ListView("LookAt Targets")][LVHeader("Target")]
    public Transform[] lookatTransform;
    [ListView("LookAt Targets")][LVHeader("Text")]
    public string lookatText;
    [ListView("LookAt Targets")][LVHeader("Sprite")]
    public Sprite lookatSprite;


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

    async void Start(){
        logStuff("Beginning intitial setup.");
        _camCount = camPosition.Length;
        _curCam = 0;

        logStuff("Setting up camera materials and render textures.");
        mainCamImage.material = previewMat;
        mainCamImage.material.mainTexture = mainCamTex;
        mainCamera.enabled = false;
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
        // Default state for FX system.
        effectsQuad.enabled = false;
        for (int i = 0; i < effectsButton.Length; i++){
            effectsButton[i].enabled = true;
        }
        logStuff("End of Startup.");

        // update owner text to show the current owner.
        ownerText.text = Networking.GetOwner(cameraController.gameObject).displayName;
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
        previewCamera.Render();
        if ( _curCam < _camCount - 1 ) {
            _curCam += 1;
        } else {
            _curCam = 0;
        }

        if (_localAutoCycle && !_localCamLocked) {
            if ( _manualOverride == true ) {
                float now = Time.unscaledTime;
                if  (now - _manualOverrideTime > delayOnOverride){
                    _manualOverride = false;
                    autoCyclePreviewSlider.value = 0.0f;
                    logStuff("Manual Override timeout elapsed - Resuming Auto Cycle.");
                } else {
                    autoCyclePreviewSlider.value = (now - _manualOverrideTime) / delayOnOverride;
                }
            }
            if ( Time.unscaledTime - _lastLocalAutoCamUpdate > _localDelayAutoCam && _manualOverride == false) {
                _lastLocalAutoCamUpdate = Time.unscaledTime;
                _localDelayAutoCam = Random.Range(autoUpdateSecondsLow, autoUpdateSecondsHigh + 1);
                autoCyclePreviewSlider.value = 1.0f;
                bool thisIsMine = Networking.IsOwner(Networking.LocalPlayer, cameraController.gameObject);
                if (thisIsMine) {
                    if (randomizeCamera ) {
                        // Random.Range Int override doesn't include the max, so Length should return 0 to Length-1
                        //      https://docs.unity3d.com/ScriptReference/Random.Range.html 
                        _localCurrentAutoCam = Random.Range(0, _autoEnabledCams.Length);
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
            } else if (!_manualOverride) {
                autoCyclePreviewSlider.value = (Time.unscaledTime - _lastLocalAutoCamUpdate) / _localDelayAutoCam; 
            }
        }
        
        // Just syncs the state of the checkbox.
        if (_lerping != _localLerping) {
            _localLerping = _lerping;
            lerpToggle.enabled = _localLerping;
        }
        if (lerpPosition && _lerping){
            float _traveled = (Time.unscaledTime - _lerpStartTime) * _lerpSpeed;
            float percentTraveled = Mathf.SmoothStep(0.0f, 1.0f, _traveled / _lerpDistanceTotal);
            mainCamera.transform.position = Vector3.Lerp(_lerpStartPosition.position, camPosition[_localActiveCam].position, percentTraveled);
            mainCamera.transform.rotation = Quaternion.Lerp(_lerpStartPosition.rotation, camPosition[_localActiveCam].rotation, percentTraveled);
            if (percentTraveled >= 1.0f) {
                // Needs to be parented and zero'd out in case of an animated camera.
                mainCamera.transform.parent = camPosition[_localActiveCam];
                mainCamera.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                mainCamera.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
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
                // Move instantly to the new location and zero out the position to account for moving cameras.
                mainCamera.transform.parent = camPosition[_localActiveCam];
                mainCamera.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                mainCamera.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                logStuff("Moved instantly to new cam position.");
            }
        }
        if (_localAutoCycle != autoCycle) {
            _localAutoCycle = autoCycle;
            autoCycleToggle.isOn = _localAutoCycle;
            // Hide/Show the preview slider based on whether AutoCam is enabled and if we're the owner.
            bool thisIsMine = Networking.IsOwner(Networking.LocalPlayer, cameraController.gameObject);
            if (thisIsMine && _localAutoCycle) {
                autoCyclePreviewSlider.gameObject.SetActive(true);
            } else {
                autoCyclePreviewSlider.gameObject.SetActive(false);
            }
            logStuff("Toggled AutoCycle to " + _localAutoCycle.ToString());
        }
        if (_localCamLocked != camLocked) {
            _localCamLocked = camLocked;
            camLockToggle.isOn = _localCamLocked;
            logStuff("Toggled Cam Locking to " + _localCamLocked.ToString());
        }
        if (_currentEffect != _localCurrentEffect){
            _localCurrentEffect = _currentEffect;
            effectsQuad.material = effectsMaterial[_localCurrentEffect];
            effectsQuad.enabled = true;
            effectsQuad.gameObject.GetComponent<Animator>().SetTrigger(effectsAnimTrigger[_localCurrentEffect]);
            logStuff("Updated FX to new setting, sending triger: " + effectsAnimTrigger[_localCurrentEffect]);
        }
    }

    public override void OnOwnershipTransferred() {
        ownerText.text = Networking.GetOwner(cameraController.gameObject).displayName;
    }

    // Toggle the position of the main camera based on the button that has had its mesh renderer disabled by a button press.
    //   If the mesh renderer is disabled (by button), re-enable it and set the activeCam to the corresponding value. 
    //   Main logic for handling the transition to the new camera position is handled in OnDeserialization() and LateUpdate() (if lerping)
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

    // Toggle the FX value based on button that was pressed - the button disables itself as a way to flag it as the one that was pressed
    //  so we find the one with the disabled button, set current effect to the corresponding value, then handle the logic for toggling
    //  in OnDeserialization() so it syncs.
    public async void toggleFX(){
        for (int i = 0; i < effectsButton.Length; i++){
            if ( effectsButton[i].enabled == false ) {
                TakeOwner();
                _currentEffect = i;
                effectsButton[i].enabled = true;
                break;
            }
        }
        RequestSerialization();
        OnDeserialization();
    }

    // Basic synced toggle for camera lock - main logic handled in OnDeserialization()
    public void lockToggle() {
        TakeOwner();
        camLocked = camLockToggle.isOn;
        RequestSerialization();
        OnDeserialization();
    }

    // Basic synced toggle for auto cycling - main logic handled in OnDeserialization()
    public void toggleAutoCam(){
        TakeOwner();
        autoCycle = autoCycleToggle.isOn;
        RequestSerialization();
        OnDeserialization();
    }

    // Basic synced toggle for camera lerping during transitions - main logic handled in OnDeserialization()
    public void toggleLerpPosition(){
        TakeOwner();
        lerpPosition = lerpToggle.isOn;
        RequestSerialization();
        OnDeserialization();
    }
}