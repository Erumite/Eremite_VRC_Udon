
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SelfDestruct : UdonSharpBehaviour
{
    [Header("Send the 'goodbyeWorld' event to this object to effectively destroy it in-scene.")]
    [Tooltip("Write Debugging to log file.")]
    public bool debugLog = false;

    [UdonSynced]
    private bool isDead = false;
    private string me;

    void Start()
    {
        me = gameObject.transform.name;
        Debug.Log("Someone set up '" + me + "' the bomb.");
    }
    private void logStuff(string message){
        if (debugLog){
            Debug.Log("[Eremite](SelfDestruct) " + message);
        }
    }

    public override void OnDeserialization(){
        if (isDead) {
            logStuff("" + me + "will be 'destroyed'.");
            // Disable all children in the game object if they exist:
            var allChildren = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren) {
                if (child.gameObject != gameObject) {
                    logStuff(" - Disabling all child object: " + child.name);
                    child.gameObject.SetActive(false);
                }
            }
            // Destroy all components in this game object except the UdonBehavior for late joiners.
            var myComponents = gameObject.GetComponents<Component>();
            foreach (Component c in myComponents){
                if (c.GetType() != typeof(UdonBehaviour) && c.GetType() != typeof(Transform)){
                    logStuff("Destroying component on this object: " + c.GetType().ToString());
                    Destroy(c);
                }
            }
        }
    }

    public void goodbyeWorld() {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        isDead = true;
        RequestSerialization();
        OnDeserialization();
    }
}