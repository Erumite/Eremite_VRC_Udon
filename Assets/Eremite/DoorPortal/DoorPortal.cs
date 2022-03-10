using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorPortal : UdonSharpBehaviour
{
    [Header("Door Portal Config")]
    [Tooltip("The VRCPortalMarker script set up with the world ID this door should lead to.")]
    public VRC_PortalMarker portal;
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
    private bool localIsLocked = false;
    [Tooltip("The value of the key required to open the door. (0 = no key value required, just an Unlock call)")]
    public int keyValue = 0;

    // optional debugging to log for dev.
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
    }

    private void logStuff(string message){
        if (debugLog == true) {
            Debug.Log("[Eremite](DoorPortal) : " + message);
        }
    }

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
            }
        } else {
            portalNameTag.gameObject.SetActive(false);
            logStuff("Disabled portal's world name.");
        }

    }

    public void Unlock() {

    }

    private void lockPortal(bool locked){
        // negate locked - if locked=true, collision=false (etc)
         portalCollider.enabled = !locked;
    }

    public void setupPortal() {
        getPortalObjects();
        hideShowPreview();
        hideShowWorldName();
        // Just hide the platform icons, I guess.  May make this an option later. 
        if ( portalPlatformIcons ) { portalPlatformIcons.gameObject.SetActive(false); }
    }
    
    private void OnPlayerTriggerEnter(VRCPlayerApi player) {
        // WIP : Eventually play special effects/etc when someone enters.
    }
}
