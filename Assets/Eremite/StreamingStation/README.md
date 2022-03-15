# Stream Station
This is for events where someone is either recording or streaming video of the world. (Podcasts, Concerts, etc). The idea is to reduce lag as much as possible and reduce possible interruptions to the streamer's video feed.

Demo World [here](https://vrchat.com/home/launch?worldId=wrld_ef6e6776-a6e6-445a-9325-5128929a86ed&instanceId=0)

View of others in the world: 

https://user-images.githubusercontent.com/44117913/158314298-a324d1f7-6700-4cfe-a24c-9161cd025980.mp4

View for the Streamer (the `"W" Get Up` message goes away after a few seconds): 

https://user-images.githubusercontent.com/44117913/158314344-1a3a45e0-4572-4036-96aa-1cff0e788213.mp4


---

### Setup

1. Drag Prefab into the scene and edit a few options:
 * `Stream Capsule Material` - Assign the material with the RenderTexture that the streamer will be capturing in the config.
 * `PostProcessing` - If you have Post Processing in the world (particularly bloom), make sure you add any of the game objects handling it to this array.  When the player is seated, they will disable the post processing locally, then re-enable it when they stand.
 * `Stream Capsule Material` - This is the material that is displayed to the streamer.  It can be swapped to another material if you need two streamers, recorders, etc.
 * If you have two streamers, you can edit the object in `Label Text Mesh` to whatever you want. (eg: "Streamer" and "Recorder")
 * Be sure to bake Occlusion Culling while the station is in the scene for occlusion to work properly.

##### Optional Config
This shouldn't need editing, but an explanation of what things are doing is in order.

* `Blocker Colliders` - This takes a list of game objects with Colliders and enables them **globally** while someone is seated in the chair, then disables on exit.  This is to keep other players out.
* `Streamer Occlusion Portal` - This is a box around the chair that will occlude **everything** in the world for the player's camera. This doesn't affect the main camera unless it moves into the occlusion portal.  This means they don't have to render everyone's models and the environment twice.
* `Stream Shader Capsule` - This is just a cube with a shader on it that takes a texture and puts it in Screen Space. It's enabled **locally** for the streamer only while seated.
* `Chair Mesh Renderer` - The chair mesh gets disabled **globally** while the streamer is seated.
* `Label Text Mesh` - The text/label for the chair gets disabled **globally** while the streamer is seated.
* The `SeatedPosition` and `ExitPosition` of the chair can be moved to wherever you want the person to be while seated and when they exit. (This might be useful if you wanted the chair in one place, but the capsules elsewhere).
* `Debug Log` - Usually want this off unless something's being screwy. It'll just log stuff to the VRC/Unity log.

##### Streamer Tips:

When streaming, you probably want to specify the screen width and height of the VRChat client, otherwise you may see black bars at the top and bottom of OBS.

Sample Windows Shortcut Target:
```
"C:\Program Files (x86)\Steam\steamapps\common\VRChat\VRChat.exe" --no-vr --enable-sdk-log-levels --enable-debug-gui --no-steam -screen-height 1080 -screen-width 1920
```
More about [Launch Options](https://docs.vrchat.com/docs/launch-options)

#### ToDo:

* Change to Blank Avatar on Station Enter
