# Door Portals
Replace VRChat's portals with a lockable door with custom effects.

Inspired by Hardlight's old Door Portals from SDK2 ToyBox - they've been broken in SDK2 for a while though.  I copied the Particle System settings from the old prefab, but the rest is re-made with SDK3.

**Demo World**: [here](https://vrchat.com/home/launch?worldId=wrld_2f380889-8506-47fa-9cff-2eb0ea2b4b73&instanceId=0)  
**Demo Video**:

https://user-images.githubusercontent.com/44117913/157834642-2c93e8d4-cc02-46c9-81e2-73399fc8babb.mp4

---

### Setup

##### DoorPortal:

The prefab is mostly set up and is drag and drop, but there are some options you might want to change.  Most are pretty self explanatory and have tooltips explaining them.

* **Show Preview** : Should it keep the original portal graphic showing the world icon?
  * **Preview Location** : Where the preview will show up at. Copies position and rotation.
  * **Preview Size** : Scale of the preview. (Multiplicative, so 1.0=unchanged)
* **Show World Name** : Should it show the text with world name/instance ID, etc?
  * **World Name Location** : Where the location text is moved to.  Copies position and rotation.
  * **World Name Size** : Scale of the world text. (Multiplicative, so 1.0=unchanged)
* **IsLocked** : Whether the door defaults to locked or not.
* **Destroy KeyHole** : Should the keyhole be destroyed after the door unlocks?
* **Lock Sound** : Swap this out to change the sound it plays when you try to open a locked door.
* **Door is Open** : Does the door default to being open? (closed by default)
* **Key Value** : Unused at the moment.

##### Keying the Door

Add a `DoorPortalKeying` script to whatever you want to use as a lock.  
Add keys to `Accepted Key Colliders`.  These are objects that can unlock the door.  
Specify the `DoorPortal` controller.  
There's a checkbox to optionally destroy the key when it is used.

##### Destroying Keys
If the checkbox to destroy key on use is checked above, it will send a networked `goodbyeWorld` event to the colliding object.  Add a `SelfDestruct` script to the key to receive the event.

This will:
* Disable all child objects
* Find all components on the key object itself.
  * If the component is not an `UdonBehavior` or `Transform`, `Destroy()` it.
  * This allows for syncing of late-joiners since the UdonBehavior still exists.

---
