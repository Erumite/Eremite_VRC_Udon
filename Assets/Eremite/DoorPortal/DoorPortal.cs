using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorPortal : UdonSharpBehaviour
{
    public VRC_PortalMarker portal;
    void Start()
    {
        hidePortalBits();
    }

    public void hidePortalBits(){
        var portalGraphics = portal.gameObject.transform.Find("PortalInternal(Clone)/PortalGraphics");
        if ( portalGraphics ) { portalGraphics.gameObject.SetActive(false) ; }
        var portalNameTag = portal.transform.Find("PortalInternal(Clone)/NameTag");
        if ( portalNameTag ) { portalNameTag.gameObject.SetActive(false) ; }
        var portalPlatformIcons = portal.transform.Find("PortalInternal(Clone)/PlatformIcons");
        if ( portalPlatformIcons ) { portalPlatformIcons.gameObject.SetActive(false) ; }
    }
    private void OnPlayerTriggerEnter(VRCPlayerApi player) {
    }
}
