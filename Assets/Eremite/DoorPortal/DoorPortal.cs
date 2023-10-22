using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DoorPortal : UdonSharpBehaviour
{
    [Header("Door Portal Options")]
    [Tooltip("Show the world image at a specified location.")]
    public bool showPreview = false;
    [Tooltip("Location and Rotation that the preview will appear.")]
    public Transform showPreviewLocation;
    [Tooltip("Scale the preview. (1.0 = normal size ; 0.1 = 10% size)")]
    public float showPreviewSize = 0.1f;
    [Tooltip("Show World Name?")]
    public bool showWorldName = false;
    [Tooltip("Where should the world name show up?")]
    public Transform showWorldNameLocation;
    [Tooltip("Optionally scale the world name.")]
    public float showWorldNameSize = 1.0f;

    [Header("Door Lock Settings")]
    [UdonSynced]
    [Tooltip("(SYNCED) If the door is locked by default, it will need to be unlocked before it can be opened and the portal used.")]
    public bool isLocked = false;
    private bool localIsLocked;
    [Tooltip("The Key Hole's game object.")]
    public GameObject keyHole;
    [Tooltip("Should the keyhole be destroyed when unlocked?")]
    public bool destroyKeyholeWhenUnlocked;
    [Tooltip("Audio to play when the door is locked and doorknob is clicked.")]
    public AudioClip lockedSound;
    [Tooltip("Audio to play when the door is unlocked.")]
    public AudioClip unlockSound;
    [Tooltip("Audio Source that handles the above clips.")]
    public AudioSource lockSoundSource;

    [UdonSynced]
    [Tooltip("Should the door be open by default? (not recommended)")]
    public bool doorIsOpen = false;
    private bool localDoorIsOpen;

    [Header("Door Portal Internals")]
    [Tooltip("This Udon Behavior")]
    public UdonBehaviour doorPortalScript;
    [Tooltip("Animator that handles the Door opening/closing.")]
    public Animator doorPortalAnimator;
    [Tooltip("The VRCPortalMarker script set up with the world ID this door should lead to.")]
    public VRC_PortalMarker portal;

    // optional debugging to log for dev.
    [Header("Debugging - Tag = (DoorPortal)")]
    public bool debugLog = false;

    // internals
    private Transform portalGraphics;
    private Transform portalNameTag;
    private Transform portalPlatformIcons;
    private Transform portalCore;
    private Transform portalFringe;
    private BoxCollider portalCollider;
    private bool internalsFound = false;
    
    // Paths of objects
    private string portalGraphicsPath = "PortalInternal(Clone)/PortalGraphics";
    private string nametagPath = "PortalInternal(Clone)/Canvas/NameTag";
    private string portalCorePath = "PortalInternal(Clone)/PortalGraphics/PortalCore";
    private string portalFringePath = "PortalInternal(Clone)/PortalGraphics/PortalFringe";
    private string platformIconsPath = "PortalInternal(Clone)/PlatformIcons";
    private string clonePath = "PortalInternal(Clone)";

    // Uniform logging message for searching debug log.
    private void logStuff(string message){
        if (debugLog == true) {
            Debug.Log("[Eremite](DoorPortal) : " + message);
        }
    }

    void Start()
    {
        #if UNITY_ANDROID
            portalGraphicsPath = "PortalInternalMobile(Clone)/PortalGraphics";
            nametagPath = "PortalInternalMobile(Clone)/Canvas/NameTag";
            portalCorePath = "PortalInternalMobile(Clone)/PortalGraphics/PortalCore";
            portalFringePath = "PortalInternalMobile(Clone)/PortalGraphics/PortalFringe";
            platformIconsPath = "PortalInternalMobile(Clone)/PlatformIcons";
            clonePath = "PortalInternalMobile(Clone)";
        #endif
        setupPortal();
        // Set the lock sound - allow editing from UdonBehavior rather than updating the audio source.
        lockSoundSource.clip = lockedSound;
        // Run update after we've set up our baseline loaded state.
        OnDeserialization();
    }

    public void setupPortal() {
        getPortalObjects();
        hideShowPreview();
        hideShowWorldName();
        // Just hide the platform icons, I guess.  May make this an option later. 
        if ( portalPlatformIcons ) { portalPlatformIcons.gameObject.SetActive(false); }
        if (portalCollider) {
            portalCollider.transform.localScale = new Vector3(1.0f,1.1f,0.1f);
            portalCollider.enabled = localDoorIsOpen;
        }
    }

    #region "Portal Setup and Toggle"
     // Set all the objects in the portal - if not found, log to debug log.
    private void getPortalObjects() {
        internalsFound = true;
        // Portal Graphics : Portal preview image and fringe particles parent.
        portalGraphics = portal.gameObject.transform.Find(portalGraphicsPath);
        if (!portalGraphics) { 
            logStuff("Couldn't find Portal Graphics transform.");
            internalsFound = false;
        }
        // Portal Name Tag : the text of the world name that shows up above the portal normally.
        portalNameTag = portal.transform.Find(nametagPath);
        if (!portalNameTag) {
            logStuff("Couldn't find Portal Name Tag transform.");
            internalsFound = false;
        }
        // PortalCore : Contains the preview image for the world.
        portalCore =  portal.transform.Find(portalCorePath);
        if (!portalCore) {
            logStuff("Couldn't find Portal Core transform.");
            internalsFound = false;
        }
        // Portal Fringe : Particle system around the edge of the portal.
        portalFringe = portal.transform.Find(portalFringePath);
        if (!portalFringe) {
            logStuff("Couldn't find Portal Fringe transform.");
            internalsFound = false;
        }
        // Platform Icons: Shows if the world is quest/PC compatible.
        portalPlatformIcons = portal.transform.Find(platformIconsPath);
        if (!portalPlatformIcons) {
            logStuff("Couldn't find the Portal Platform Icons transform.");
            internalsFound = false;
        }
        // Portal Collider = the collider that you interact with to enter the portal.
        var portalColliderObject = portal.transform.Find(clonePath);
        if (portalColliderObject) {
            portalCollider = portalColliderObject.GetComponent<BoxCollider>();
            if (!portalCollider) {
                logStuff("Found PortalInternal, but couldn't find its box collider.");
                internalsFound = false;
            }
        } else {
            logStuff("Couldn't find the PortalInternal(Clone) game object.");
            internalsFound = false;
        }
    }


    private void hideShowPreview() {
        // Make sure the portal core transform is not null.
        if (portalCore && portalGraphics) {
            if ( showPreview == true ) {
                // Move core to position/rotation and modify the scale.
                ///  F*** It - Disabling mesh renderer at startup is broken, so just move it to Timbuktu.
                portalCore.SetPositionAndRotation(new Vector3(9999.0f,9999.0f,9999.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f));
                portalCore.localScale = portalCore.localScale * showPreviewSize;
                logStuff("Moved portal world preview to its new position.  Opened: " + localDoorIsOpen.ToString());
               // disable the fringe particle system
                if ( portalFringe ) { portalFringe.gameObject.SetActive(false); }
                
                logStuff("Disabled Portal Fringe");
            } else { 
            // otherwise, just disable all this stuff.
            portalGraphics.gameObject.SetActive(false);
            logStuff("Disabled portal graphics / preview.");
            }
        }
    }

    private void hideShowWorldName() {
        // If we want to show the name, move it to the right location/scale.
        if (portalNameTag) {
            if (showWorldName == true ) { 
                portalNameTag.SetPositionAndRotation(showWorldNameLocation.position, showWorldNameLocation.rotation);
                portalNameTag.localScale = portalNameTag.localScale * showWorldNameSize;
                portalNameTag.gameObject.SetActive(localDoorIsOpen);
                logStuff("Moved portal world name to its new position.");
            } else {
            portalNameTag.gameObject.SetActive(false);
            logStuff("Disabled portal's world name.");
            }
        }
    }

    #endregion "Portal Setup and Toggle"
    #region "Networking Stuff"

    public override void OnDeserialization(){
        if ( ! internalsFound ) {
            getPortalObjects();
        }
        if ( localIsLocked != isLocked ){
            if ( isLocked == false && localIsLocked == true ) {
                lockSoundSource.PlayOneShot(unlockSound);
            }
            localIsLocked = isLocked;
        }
        // Destroy the keyhole if it's unlocked and the option is checked.
        if (keyHole && !localIsLocked && destroyKeyholeWhenUnlocked){
            Destroy(keyHole);
        }
        if ( localDoorIsOpen != doorIsOpen ){
            localDoorIsOpen = doorIsOpen;
        }
        if (!localIsLocked) {
            OpenDoor(localDoorIsOpen);
        }
        logStuff("OnDeserialization: Locked=(L:" + localIsLocked.ToString() + "/G:" + isLocked.ToString() + ") | Open=(L:" + localDoorIsOpen.ToString() + "/G:" + doorIsOpen.ToString() +")");
    }
    public void Unlock() {
        Networking.SetOwner(Networking.LocalPlayer, doorPortalScript.gameObject);
        isLocked = false;
        RequestSerialization();
        OnDeserialization();
    }

    #endregion "Networking Stuff"
    #region "Portal Locking"

    #endregion "Portal Locking"
    # region "Door open and close" 

    // Trigger animations and whatnot for door opening and closing
    public void turnDoorKnob() {
        if ( !localDoorIsOpen && localIsLocked) {
            // Toggle off/on with auto-play to avoid OneShot spam.
            lockSoundSource.gameObject.SetActive(false);
            lockSoundSource.gameObject.SetActive(true);
            logStuff("Door is locked - playing lock sound.");
        } else {

            Networking.SetOwner(Networking.LocalPlayer, doorPortalScript.gameObject);
            // if open, this closes it  and vice versa
            doorIsOpen = !doorIsOpen;
            logStuff("Opening Door: " + doorIsOpen.ToString());
            RequestSerialization();
            OnDeserialization();
        }
        logStuff("TurnDoorKnob: Locked=(L:" + localIsLocked.ToString() + "/G:" + isLocked.ToString() + ") | Open=(L:" + localDoorIsOpen.ToString() + "/G:" + doorIsOpen.ToString() +")");
    }

    public void OpenDoor(bool opened){
        doorPortalAnimator.SetBool("Open", opened);
        if  (portalCollider ) {
            portalCollider.enabled = opened;
        }
        // If the preview/name are enabled, turn them off when closed, on when opened.
        if (showPreview) {
            // Can't disable mesh renderer at startup, so it gets moved.  Move it back to the right spot.
            if (portalCore){
                portalCore.SetPositionAndRotation(showPreviewLocation.position, showPreviewLocation.rotation);
                // Disable the mesh instead - otherwise the preview image seems to break.
                var pcMesh = portalCore.gameObject.GetComponent<MeshRenderer>();
                if (pcMesh) { pcMesh.enabled = opened; }
            }
        }
        if (showWorldName && portalNameTag != null) {
            portalNameTag.gameObject.SetActive(opened);
        }
    }
    

    #endregion "Door open and close"


    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player) {
        doorPortalAnimator.SetTrigger("EnterPortal");
    }
}
