using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// A button for interacting with the Keypad Controller
// This can be any 3D game object with a collider for interaction.
// Must contain a child object named "pressed" - ideally an empty object.

public class Keypad3DButton : UdonSharpBehaviour
{
    [Tooltip("The KeyPad controller that this will send a message to.")]
    public UdonBehaviour keypadController;

    // Ensure that the "pressed" game object is disabled when starting up.
    void Start()
    {
        transform.Find("pressed").gameObject.SetActive(false);
    }

    // On interaction, set the "pressed" object to active so the controller can
    //  tell which button was pressed.  Then send a message to the controller to
    //  process the interaction.
    public override void Interact(){
        transform.Find("pressed").gameObject.SetActive(true);
        keypadController.SendCustomEvent("KeyPress");
    }
}
