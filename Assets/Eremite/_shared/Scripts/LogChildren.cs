using System;
using System.ComponentModel.Design;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509.Qualified;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

// This is some hacky crap to debug certain situations where you need to list the heirarchy in-game since it can't be done in the editor.
//  Originally meant to enumerate all the items in a VRC_PortalMarker after it has had its children populated by the VRC Client.

public class LogChildren : UdonSharpBehaviour
{
    public GameObject objectToInspect;
    public Text textArea;
    public bool listChildenAtStart = true;

    void Start()
    {
        if (listChildenAtStart){
            listChilden();
        }
    }
    public void listChilden(){
        clearTextArea();
        var allChildren = objectToInspect.GetComponentsInChildren<Transform>();
        foreach ( Transform child in allChildren ) {
            GetGameObjectPath(child.gameObject);
            logToTextArea("* Position: " + child.transform.localPosition.ToString());
            logToTextArea("* Rotation: " + child.transform.localRotation.ToString());
            logToTextArea("*    Scale: " + child.transform.localScale.ToString());
            Component[] components = child.GetComponents(typeof(Component));
            foreach(Component component in components) {
                if (component != null){
                    if (component.GetType() != typeof(Transform)) {
                        logToTextArea(" - Component: " + component.ToString());
                    }
                }
            }
        }
    }

    public void clearTextArea(){
        if (textArea != null) {
            textArea.text = "";
        }
    }

    public void logToTextArea(string s){
        Debug.Log(s);
        if (textArea != null) {
            textArea.text = textArea.text + s + "\n";
        }
    }

    // https://answers.unity.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
    private void GetGameObjectPath(GameObject obj) {
        string path = "/" + obj.name;
        while (obj.transform.parent != null) {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        Debug.Log(path);
        logToTextArea(path);
    }
}
