
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

//  Manage debug logging for a whole scene at once instead of having to toggle multiple objects.
//   * Debug logging can be enabled by other scripts to turn on logging for an uploaded world.
//   * The header is appended for grepping/searching the relevant lines from the VRC output log.
//   * Including class and game object names is useful for narrowing down the behavior with problems.
//
//  Example debugging in format:  (World Name) [Udon Class] {Game Object Name} Message goes here.

// public class MyClassThatDoesCoolStuff : UdonSharpBehaviour {
//     public DebugLoggingManager debugger;
//     private string debugString;
//
//     void Start() {
//         debugString = $"[{this.GetUdonTypeName()}] <{this.name}> " ;
//     }
//     private void logDebug(string s){
//          if (debugger == null){return;}
//          debugger.Log($"{debugString}{s}");
//     }
//     private void logWarning(string s){
//          if (debugger == null){return;}
//          debugger.Log($"{debugString}{s}");
//     }
// }

//  Send a message to the debugging function:
//      debugLog("a thing has happened.");

//  Enable debugging from script:
//      debugger.enableDebugging(true);

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DebugLoggingManager : UdonSharpBehaviour
{
    [Tooltip("Appended to the very start of the debug log - typically world name.  Useful for searching for all events from a world.")]
    [SerializeField]
    private string header = "My World";
    [Tooltip("Enable debugging on all objects referencing this script.")]
    public bool logDebug = false;
    [Tooltip("Enable warnings on all objects referencing this script.")]
    public bool logWarnings = false;

    void Start(){
        header = $"({header}) ";
        Log($"Debugger started!   Debugging: {logDebug} | Warnings: {logWarnings}");
    }

    public void Log(string s){
        if (logDebug) { Debug.Log(header + s); }
    }
    public void Warn(string s){
        if (logWarnings) { Debug.LogWarning(header + s); }
    }

    // These can be referenced by other script to enable debugging in world if it's off by default.
    public void enableDebugging(bool b){
        logDebug = b;
    }
    public void enableWarnings(bool b){
        logWarnings = b;
    }
}