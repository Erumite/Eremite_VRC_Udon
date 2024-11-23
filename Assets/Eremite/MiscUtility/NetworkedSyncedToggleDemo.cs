
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// Technically this is probably useful for setting up a synced toggle that will
//   toggle an item off/on for everyone in an instance and also keep synced
//   for late joiners, but the intent is to show how to handle networking with U#
//   using OnDeserialization for the logic and synced + local variables.

public class NetworkedSyncedToggleDemo : UdonSharpBehaviour
{
    // is_on is a variable synced over the network and is updated by
    //  the owner of this object.  The field is serialized so it shows in
    //  the object inspector so we can set a default state.
    [UdonSynced]
    [SerializeField] 
    private bool _is_on = false;
    // _is_on_local is a duplicate of the above for tracking the last
    //   state that we got an update for (last value received).
    // Lets us avoid excess logic for performance.
    private bool _is_on_local;

    // The game object that will be toggled by this script.
    [SerializeField]
    private GameObject toggleObject;

    void Start()
    {
        // The object should start in the default state from the Object Inspector checkbox.
        toggleObject.SetActive(_is_on);
        // Set our local value to match default state so we know whether to process incoming events.
        _is_on_local = _is_on;
    }
    
    // All the logic around changing things in th world should happen here if it needs to
    //   be synced with everyone and for late joiners.
    public override void OnDeserialization(){
        // To avoid complex logic when not necessary, we only perform the
        // bracketed code when there's an update - local variable does not match
        // the synced variable.
        if (_is_on != _is_on_local)
        {
            // Update local value with the value of the synced value.
            _is_on_local = _is_on;
            // Toggle the object based on our newly synced bool.  Either bool is fine here.
            toggleObject.SetActive(_is_on_local);
        }
    }

    // This is public so that it can be called from an Interact object or wherever.
    // Note that no toggling of objects is happening here, only updating the synced
    //   variable so that OnDeserialization can do its thing.
    public void ToggleObject()
    {
        // If we're not the owner of this object, return since we don't have the permission
        //   to update synced variables.  Not strictly necessary, but can save some logic overhead
        //   for non-owners if more than a simple toggle is happening.
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) { return; }
        // Toggle the value of _is_on
        _is_on = !_is_on;
        // If on manual sync mode, send an update to everyone in instance.
        RequestSerialization();
        // We also want to call OnDeserialization ourselves as we won't receive updated values from ourselves.
        OnDeserialization();
    }
}
