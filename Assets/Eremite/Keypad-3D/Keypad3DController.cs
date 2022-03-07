﻿using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Keypad3DController : UdonSharpBehaviour
{
    public AudioSource pressAudioSource;
    // Buttons that interact with the Keypad Controller
    // The value added to the password corresponds with the name of the game object.
    // Special phrases apply: (eg: -CLR- or -RETURN-) :  See docs
    [ListView("Button Config")][LVHeader("Button")]
    public GameObject[] buttons;
    [ListView("Button Config")][LVHeader("PressSound")]
    public AudioClip[] pressSound;

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

    // Don't really need to do anything to start.
    void Start() {}
    
    // If debugging is checked, this outputs to VRChat's log and the Unity Console
    private void logStuff( string message ){
        if ( logToDebug == true ) {
            Debug.Log("[Eremite](3DKeyPad) " + message);
        }
    }

    // When the return key is pressed, check the current password vs the list of passwords.
    // If a match is found, send the event or network event and reset the password field.
    private async void processPassword(string pass) {
        logStuff("Committing password attempt: " + pass);
        for (int i=0; i<passWord.Length; i++){
            if ( passWord[i] == passwordEntry ) {
                UdonBehaviour t = (UdonBehaviour)udonTarget[i].GetComponent(typeof(UdonBehaviour));
                if ( isNetworked[i] == true ) {
                    t.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, udonAction[i]);
                } else {
                    t.SendCustomEvent(udonAction[i]);
                }
                break;
            }
        }
        passwordEntry = string.Empty;
    }

    // We can't tell which object sent the message, so we select the one with the 'pressed' child object enabled
    // and get its name.  Match any special commands if there, otherwise append the sending object's name to password.
    public async void KeyPress(){
        logStuff("Key Pressed!");
        string newText = string.Empty;
        for ( int b=0 ; b < buttons.Length ; b++ ) { 
            if ( buttons[b].transform.Find("pressed").gameObject.activeInHierarchy == true ) {
                buttons[b].transform.Find("pressed").gameObject.SetActive(false);
                newText=buttons[b].name;
                pressAudioSource.PlayOneShot(pressSound[b]);
                break;
            }
        }
        if ( newText == "-CLR-") {
            passwordEntry = string.Empty;
        } else if ( newText == "-RETURN-" ) {
            processPassword(passwordEntry);
        } else {
            passwordEntry += newText;
            logStuff("New Text: " + newText);
            logStuff("Password So Far: " + passwordEntry);
        }
    }
}