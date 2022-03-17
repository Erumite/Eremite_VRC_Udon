# Event Camera System

### Under Construction

A method of handling the main camera angle for podcast recording, concerts and more.

Inspired by [Scyne](https://www.twitch.tv/scynewave)'s camera system for his podcast. (check it out)  
Created for handling the Cameras and FX for [Nyx's Abyss](https://www.twitch.tv/nyxsabyss) concerts.

For a way to capture the MainCam output for streaming or recording, see the [Streaming Station](/Assets/Eremite/StreamingStation).

---

(documentation incoming)

---

#### ToDo:

* SmoothStep Transitions between points while LERPing: https://docs.unity3d.com/ScriptReference/Mathf.SmoothStep.html
* Alternate Transitions: Only one type active at once.
  * Screen Wipe between old/new cam - use Preview rendertex for OldCam to avoid performance hit of two cameras.
