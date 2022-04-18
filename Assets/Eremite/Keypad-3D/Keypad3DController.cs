using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Keypad3DController : UdonSharpBehaviour
{
    [Tooltip("This is the source where the audio for keypresses play from.")]
    public AudioSource pressAudioSource;
    [Tooltip("If false, it will check against existing passwords on each keypress and must be cleared with a -CLR- button.\nIf true, a -RETURN- button triggers checking passwords and clears entered characters on success or failure.")]
    public bool requireEnterKeyToSubmit = true;
    // Buttons that interact with the Keypad Controller
    // The value added to the password corresponds with the name of the game object.
    // Special phrases apply: (eg: -CLR- or -RETURN-) :  See docs
    [ListView("Button Config")][LVHeader("Button")]
    public GameObject[] buttons;
    [ListView("Button Config")][LVHeader("PressSound")]
    public AudioClip[] pressSound;
    [ListView("Button Config")][LVHeader("All Hear")]
    public bool[] pressSoundNetworked;

    // Enter a password to match, a target UdonBehavior('s game object), and the event name. Can be networked.
    [Header("Passwords/Action Pairs")]
    [ListView("PasswordActions")][LVHeader("Password")]
    public string[] passWord;
    // UdonBehavior[] is broken: https://feedback.vrchat.com/vrchat-udon-closed-alpha-bugs/p/589-udonbehaviour-array-type-is-not-defined
    [ListView("PasswordActions")][LVHeader("Udon Behavior")]
    public GameObject[] udonTarget;
    [ListView("PasswordActions")][LVHeader("Udon Action")]
    public string[] udonAction;
    [ListView("PasswordActions")][LVHeader("Networked")]
    public bool[] isNetworked;

    // Internal and debug variables.
    private string passwordEntry = string.Empty;
    public bool logToDebug = false;
    [UdonSynced]
    private int networkedClip;
    private int networkedClipLocal;
    private UdonBehaviour ub;

    void Start() {
        ub = (UdonBehaviour) gameObject.GetComponent(typeof(UdonBehaviour));
    }
    
    // If debugging is checked, this outputs to VRChat's log and the Unity Console
    private void logStuff( string message ){
        if ( logToDebug == true ) {
            Debug.Log("[Eremite](3DKeyPad) " + message);
        }
    }

    // When the return key is pressed, check the current password vs the list of passwords.
    // If a match is found, send the event or network event and reset the password field.
    private void processPassword(string pass) {
        logStuff("Committing password attempt: " + pass);
        for (int i=0; i<passWord.Length; i++){
            if ( passWord[i] == passwordEntry ) {
                logStuff("Matched a password!");
                UdonBehaviour t = (UdonBehaviour)udonTarget[i].GetComponent(typeof(UdonBehaviour));
                if ( isNetworked[i] == true ) {
                    logStuff("Sending networked event.");
                    t.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, udonAction[i]);
                } else {
                    logStuff("Sending local event.");
                    t.SendCustomEvent(udonAction[i]);
                }
                break;
            }
        }
    }

    public override void OnDeserialization()
    {
        // If the clip has changed, play a press sound.
        if (networkedClip != networkedClipLocal) {
            playPressSound();
        }
        networkedClipLocal = networkedClip;
    }

    // We can't tell which object sent the message, so we select the one with the 'pressed' child object enabled
    // and get its name.  Match any special commands if there, otherwise append the sending object's name to password.
    public void KeyPress(){
        logStuff("Key Pressed!");
        string newText = string.Empty;
        for ( int b=0 ; b < buttons.Length ; b++ ) { 
            if ( buttons[b].GetComponent<Collider>().enabled == false ) {
                buttons[b].GetComponent<Collider>().enabled = true;
                newText=buttons[b].name;
                if (pressSoundNetworked[b]) {
                    Networking.SetOwner(Networking.LocalPlayer, ub.gameObject);
                    // Shitty hack to allow networkedClip to change even if it's the same button pressed.
                    if (networkedClip == b){
                        networkedClip = b + 9999;
                    } else {
                        networkedClip=b;
                    }
                    RequestSerialization();
                    OnDeserialization();
                } else {
                    pressAudioSource.PlayOneShot(pressSound[b]);
                }
                break;
            }
        }
        if ( newText == "-CLR-") {
            passwordEntry = string.Empty;
        } else if ( newText == "-RETURN-" ) {
            processPassword(passwordEntry);
            passwordEntry = string.Empty;
        } else {
            passwordEntry += newText;
            logStuff("Password So Far: " + passwordEntry + "  | +(" + newText + ")");
        }
        if ( requireEnterKeyToSubmit == false ) {
            processPassword(passwordEntry);
        }
    }

    public void playPressSound(){
        int clip = networkedClip;
        // Remove the 9999 hack for same-keypress.
        if ( clip >= 9999 ) { clip=clip-9999; }
        pressAudioSource.PlayOneShot(pressSound[clip]);
    }
}