# Anatomy of a Portal
This is what a portal looks like in game after the client generates the internal game objects, assuming the root object is named "Portal".

Generated using [this UdonBehaviour](/Assets/Eremite/_shared/Scripts/LogChildren.cs)

These can be interacted with via Udon# or Animators.

```
/DoorPortal/Portal
* Position: (-0.4, 0.0, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.0, 1.0)
- Component: Portal (UnityEngine.Transform)
- Component: Portal (VRC.SDK3.Components.VRCPortalMarker)

/DoorPortal/Portal/preview_image
* Position: (0.6, 1.4, 0.0)
* Rotation: (-0.7, 0.0, 0.0, 0.7)
*    Scale: (1.0, 1.0, 1.0)
- Component: preview_image (UnityEngine.Transform)

/DoorPortal/Portal/world_name
* Position: (0.0, 2.5, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.0, 1.0)
- Component: world_name (UnityEngine.Transform)

/DoorPortal/Portal/PortalInternal(Clone)
* Position: (0.0, 0.0, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.1, 0.1)
- Component: PortalInternal(Clone) (UnityEngine.Transform)
- Component: PortalInternal(Clone) (Cysharp.Threading.Tasks.Triggers.AsyncDestroyTrigger)
- Component: PortalInternal(Clone) (UnityEngine.BoxCollider)

/DoorPortal/Portal/PortalInternal(Clone)/PortalGraphics
* Position: (0.0, 0.0, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.0, 1.0)
- Component: PortalGraphics (UnityEngine.Transform)

/DoorPortal/Portal/PortalInternal(Clone)/PortalGraphics/PortalCore
* Position: (9999.4, 9089.4, 9999.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (0.1, 0.0, 0.2)
- Component: PortalCore (UnityEngine.Transform)
- Component: PortalCore (UnityEngine.MeshFilter)
- Component: PortalCore (UnityEngine.MeshRenderer)

/DoorPortal/Portal/PortalInternal(Clone)/Canvas
* Position: (0.0, 2.1, 0.0)
* Rotation: (0.2, -0.5, 0.1, 0.9)
*    Scale: (1.0, 1.0, 1.0)
- Component: Canvas (UnityEngine.RectTransform)
- Component: Canvas (UnityEngine.Canvas)

/DoorPortal/Portal/PortalInternal(Clone)/Canvas/NameTag
* Position: (0.0, 0.1, 0.0)
* Rotation: (0.0, 1.0, 0.0, 0.0)
*    Scale: (1.0, 1.0, 1.0)
- Component: NameTag (UnityEngine.RectTransform)
- Component: NameTag (UnityEngine.CanvasRenderer)
- Component: NameTag (TMPro.TextMeshProUGUI)
```
