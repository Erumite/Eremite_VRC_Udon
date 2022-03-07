using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// Just a very simple demo script for showing the 3D keypad in action in the demo scene.

public class KeyPad3DDemoToggleCube : UdonSharpBehaviour
{
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public void toggleOff() {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
    public void toggleOn(){
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
