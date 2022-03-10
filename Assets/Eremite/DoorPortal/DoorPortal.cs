using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorPortal : UdonSharpBehaviour
{
    [Header("Door Portal Config")]
    [Tooltip("This Udon Behavior")]
    public UdonBehaviour doorPortalScript;
    [Tooltip("Animator that handles the Door opening/closing.")]
    public Animator doorPortalAnimator;
    [Tooltip("The VRCPortalMarker script set up with the world ID this door should lead to.")]
    public VRC_PortalMarker portal;
    [Tooltip("The doorknob - object that is interacted with to open/close the door.")]
    public BoxCollider doorknobCollider;
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

    [UdonSynced]
    [Tooltip("(SYNCED) If the door is locked by default, it will need to be unlocked before it can be opened and the portal used.")]
    public bool isLocked = false;
    private bool localIsLocked;
    [Tooltip("Audio to play when the door is locked and doorknob is clicked.")]
    public AudioClip lockSound;
    [Tooltip("Audio Source that handles the above clip.")]
    public AudioSource lockSoundSource;

    [UdonSynced]
    [Tooltip("Should the door be open by default? (not recommended)")]
    public bool doorIsOpen = false;
    private bool localDoorIsOpen;

    [Tooltip("The value of the key required to open the door. (0 = no key value required, just an Unlock call)")]
    public int keyValue = 0;

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


    void Start()
    {
        setupPortal();
        // Set the lock sound - allow editing from UdonBehavior rather than updating the audio source.
        lockSoundSource.clip = lockSound;
    }


    // Uniform logging message for searching debug log.
    private void logStuff(string message){
        if (debugLog == true) {
            Debug.Log("[Eremite](DoorPortal) : " + message);
        }
    }

    #region "Portal Setup and Toggle"
     // Set all the objects in the portal - if not found, log to debug log.
    private void getPortalObjects() {
        // Portal Graphics : Portal preview image and fringe particles parent.
        portalGraphics = portal.gameObject.transform.Find("PortalInternal(Clone)/PortalGraphics");
        if (!portalGraphics) { logStuff("Couldn't find Portal Graphics transform.");}
        // Portal Name Tag : the text of the world name that shows up above the portal normally.
        portalNameTag = portal.transform.Find("PortalInternal(Clone)/NameTag");
        if (!portalNameTag) { logStuff("Couldn't find Portal Name Tab transform.");}
        // PortalCore : Contains the preview image for the world.
        portalCore =  portal.transform.Find("PortalInternal(Clone)/PortalGraphics/PortalCore");
        if (!portalCore) { logStuff("Couldn't find Portal Core transform.");}
        // Portal Fringe : Particle system around the edge of the portal.
        portalFringe = portal.transform.Find("PortalInternal(Clone)/PortalGraphics/PortalFringe");
        if (!portalFringe) { logStuff("Couldn't find Portal Fringe transform.");}
        // Platform Icons: Shows if the world is quest/PC compatible.
        portalPlatformIcons = portal.transform.Find("PortalInternal(Clone)/PlatformIcons");
        if (!portalPlatformIcons) { logStuff("Couldn't find the Portal Platform Icons transform.");}
        // Portal Collider = the collider that you interact with to enter the portal.
        var portalColliderObject = portal.transform.Find("PortalInternal(Clone)");
        if (portalColliderObject) {
            portalCollider = portalColliderObject.GetComponent<BoxCollider>();
            if (!portalCollider) { logStuff("Found PortalInternal, but couldn't find its box collider.");}
        } else {
            logStuff("Couldn't find the PortalInternal(Clone) game object.");
        }
    }


    private void hideShowPreview() {
        // Make sure the portal core transform is not null.
        if (portalCore && portalGraphics) {
            if ( showPreview == true ) {
                // Move core to position/rotation and modify the scale.
                portalCore.SetPositionAndRotation(showPreviewLocation.position, showPreviewLocation.rotation);
                portalCore.localScale = portalCore.localScale * showPreviewSize;
                logStuff("Moved portal world preview to its new position.");
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
        if ( localIsLocked != isLocked ){
            localIsLocked = isLocked;
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
        localIsLocked = false;
        isLocked = false;
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
            if(!localDoorIsOpen) {
                doorIsOpen = !doorIsOpen;
                logStuff("Opening Door");
            } else if (localDoorIsOpen) {
                doorIsOpen = !doorIsOpen;
                logStuff("Closing Door.");
            }
            localDoorIsOpen = doorIsOpen;
            RequestSerialization();
            OpenDoor(localDoorIsOpen);
        }
        logStuff("TurnDoorKnob: Locked=(L:" + localIsLocked.ToString() + "/G:" + isLocked.ToString() + ") | Open=(L:" + localDoorIsOpen.ToString() + "/G:" + doorIsOpen.ToString() +")");
    }

    public void OpenDoor(bool opened){
        doorPortalAnimator.SetBool("Open", opened);
        portalCollider.enabled = opened;
    }
    

    #endregion "Door open and close"

    public void setupPortal() {
        getPortalObjects();
        hideShowPreview();
        hideShowWorldName();
        // Just hide the platform icons, I guess.  May make this an option later. 
        if ( portalPlatformIcons ) { portalPlatformIcons.gameObject.SetActive(false); }
        portalCollider.transform.localScale = new Vector3(1.0f,1.1f,0.1f);
        portalCollider.enabled = localDoorIsOpen;
    }
    
    private void OnPlayerTriggerEnter(VRCPlayerApi player) {
        // WIP : Eventually play special effects/etc when someone enters.
    }
}
