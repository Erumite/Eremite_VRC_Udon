using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Keypad3DButton : UdonSharpBehaviour
{
    [Tooltip("The KeyPad controller that this will send a message to.")]
    public UdonBehaviour keypadController;
    void Start()
    {
        transform.Find("pressed").gameObject.SetActive(false);
    }

    public override void Interact(){
        transform.Find("pressed").gameObject.SetActive(true);
        keypadController.SendCustomEvent("KeyPress");
    }
}
