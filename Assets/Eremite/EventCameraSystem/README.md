# Event Camera System

### Under Construction

[Demo World](https://vrchat.com/home/launch?worldId=wrld_b1536ede-7068-4834-a844-2bf4f0202369&instanceId=0)

A method of handling the main camera angle for podcast recording, concerts and more.

Inspired by [Scyne](https://www.twitch.tv/scynewave)'s camera system for his podcast. (check it out)  
Created for handling the Cameras and FX for [Nyx's Abyss](https://www.twitch.tv/nyxsabyss) concerts.

For a way to capture the MainCam output for streaming or recording, see the [Streaming Station](/Assets/Eremite/StreamingStation).

---

(documentation incoming)

---

#### ToDo:
* Alternate Transitions: Only one type active at once.
  * Screen Wipe between old/new cam - use Preview rendertex for OldCam to avoid performance hit of two cameras.
* Look-At support (camera always faces a specific target)
* Way to tell when the hand-held cam is being used (red light)
* Optional minor camera wiggle/drift when at rest in position - animator is togglable.

# Maybe to do?
* Allowlist for users who are able to interact with the camera.
  * Lock/Unlock option to allow players to interact with controls.
  * It may be best to just leave the game object disabled and enable it elsewhere via a password.
* Optionally disallow preview for Main and/or preview cameras.
  * Better performance if you're not running the cameras.
  * Option to leave preview on if say, a guest on a show needs to see themselves, but don't want to render the more expensive main camera.
