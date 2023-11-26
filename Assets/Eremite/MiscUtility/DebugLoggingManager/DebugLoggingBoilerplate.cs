
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DebugLoggingBoilerplate : UdonSharpBehaviour
{
    [SerializeField]
    private DebugLoggingManager debugger;
    private string debugString;

    void Start() {
        debugString = $"[{this.GetUdonTypeName()}] <{this.name}> " ;
        logDebug("Finished running startup!");
        debugger.enableDebugging(false);
        logDebug("I won't log this message at all.");
        debugger.enableDebugging(true);
        logDebug("I will log this message to console.");
        logWarning("This is a warning!!!");
    }

    private void logDebug(string s){
         if (debugger == null){return;}
         debugger.Log($"{debugString}{s}");
    }
    private void logWarning(string s){
         if (debugger == null){return;}
         debugger.Warn($"{debugString}{s}");
    }
}
