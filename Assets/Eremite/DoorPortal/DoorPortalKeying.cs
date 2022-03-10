using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DoorPortalKeying : UdonSharpBehaviour
{
    [Header("Config")]
    [Tooltip("(UdonBehavior) The main controller for the door portals.")]
    public UdonBehaviour doorPortalController;
    [Tooltip("A list of game objects that can open this door on collision.")]
    public GameObject[] acceptedKeyColliders;
    [Tooltip("Consume key on use?  Item will be destroyed.")]
    public bool destroyKeyOnUse = true;
    [Header("Debugging - Tag = (DoorPortalKeying)")]
    public bool debugLog = false;

    void Start()
    {
        
    }
    private void logStuff(string message){
        if (debugLog == true) {
            Debug.Log("[Eremite](DoorPortalKeying) : " + message);
        }
    }

    void OnCollisionEnter(Collision collision){
        logStuff("Collision detected.  Checking to see if collider is in the list of keys.");
        foreach(GameObject c in acceptedKeyColliders) {
          if (collision.gameObject == c){
            doorPortalController.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Unlock");
            logStuff("Found key in the list. Door will unlock!");
            if (destroyKeyOnUse == true) {
                Destroy(collision.gameObject);
                logStuff("Key was set to destroy on use.  Nuking it.");
            }
            break;
          }
          logStuff("Collider was not in the list of accepted keys. Ignoring it from now on.");
          Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collision.collider);
        }
    }
}
