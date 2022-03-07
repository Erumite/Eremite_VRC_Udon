using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Keypad3DController : UdonSharpBehaviour
{
    // Buttons that interact with the Keypad Controller
    // The value added to the password corresponds with the name of the game object.
    // Special phrases apply: (eg: -CLR- or -RETURN-) :  See docs
    [ListView("Button Config")][LVHeader("Button")]
    public GameObject[] buttons;
    [ListView("Button Config")][LVHeader("PressSound")]
    public AudioClip[] pressSound;

    [Header("Passwords/Action Pairs")]
    [ListView("PasswordActions")][LVHeader("Password")]
    public string[] passWord;
    // UdonBehavior[] is broken: https://feedback.vrchat.com/vrchat-udon-closed-alpha-bugs/p/589-udonbehaviour-array-type-is-not-defined
    [ListView("PasswordActions")][LVHeader("Udon Behavior")]
    public GameObject[] udonTarget;
    [ListView("PasswordActions")][LVHeader("Udon Action")]
    public string[] udonAction;
    [ListView("PasswordActions")][LVHeader("Networked?")]
    public bool[] isNetworked;

    // Internal variables.
    private string passwordEntry = string.Empty;
    void Start() {}
    
    private void logStuff( string message ){
        Debug.Log("[Eremite](3DKeyPad) " + message);
    }
    public void KeyPress(){
        logStuff("Key Pressed!");
    }
}