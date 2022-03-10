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

/DoorPortal/Portal/PortalInternal(Clone)
* Position: (0.0, 0.0, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.1, 1.0)
 - Component: PortalInternal(Clone) (UnityEngine.Transform)
 - Component: PortalInternal(Clone) (UnityEngine.BoxCollider)

/DoorPortal/Portal/PortalInternal(Clone)/PortalGraphics
* Position: (0.0, 0.0, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.0, 1.0)
 - Component: PortalGraphics (UnityEngine.Transform)

/DoorPortal/Portal/PortalInternal(Clone)/PortalGraphics/PortalCore
* Position: (0.0, 1.1, 0.0)
* Rotation: (0.7, 0.0, 0.0, -0.7)
*    Scale: (1.3, 0.2, 1.8)
 - Component: PortalCore (UnityEngine.Transform)
 - Component: PortalCore (UnityEngine.MeshFilter)
 - Component: PortalCore (UnityEngine.MeshRenderer)

/DoorPortal/Portal/PortalInternal(Clone)/PortalGraphics/PortalFringe
* Position: (0.0, 1.1, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (0.7, 1.0, 1.0)
 - Component: PortalFringe (UnityEngine.Transform)
 - Component: PortalFringe (UnityEngine.ParticleSystem)
 - Component: PortalFringe (UnityEngine.ParticleSystemRenderer)

/DoorPortal/Portal/PortalInternal(Clone)/NameTag
* Position: (0.0, 2.1, 0.0)
* Rotation: (0.1, -0.6, 0.1, 0.8)
*    Scale: (1.0, 1.0, 1.0)
 - Component: NameTag (UnityEngine.Transform)

/DoorPortal/Portal/PortalInternal(Clone)/NameTag/TextMesh
* Position: (0.0, 0.1, 0.0)
* Rotation: (0.0, 1.0, 0.0, 0.0)
*    Scale: (1.0, 1.0, 1.0)
 - Component: TextMesh (UnityEngine.RectTransform)
 - Component: TextMesh (UnityEngine.MeshRenderer)
 - Component: TextMesh (UnityEngine.CanvasRenderer)
 - Component: TextMesh (TMPro.TextMeshPro)
 - Component: TextMesh (UnityEngine.MeshFilter)

/DoorPortal/Portal/PortalInternal(Clone)/PlatformIcons
* Position: (0.0, 0.0, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (1.0, 1.0, 1.0)
 - Component: PlatformIcons (UnityEngine.Transform)

/DoorPortal/Portal/PortalInternal(Clone)/PlatformIcons/Quad
* Position: (0.6, 1.9, 0.0)
* Rotation: (0.0, 0.0, 0.0, 1.0)
*    Scale: (0.3, 0.3, 0.2)
 - Component: Quad (UnityEngine.Transform)
 - Component: Quad (UnityEngine.MeshFilter)
 - Component: Quad (UnityEngine.MeshRenderer)

/DoorPortal/Portal/PortalInternal(Clone)/PlatformIcons/Quad (1)
* Position: (-0.6, 1.9, 0.0)
* Rotation: (0.0, 1.0, 0.0, 0.0)
*    Scale: (0.3, 0.3, 0.2)
 - Component: Quad (1) (UnityEngine.Transform)
 - Component: Quad (1) (UnityEngine.MeshFilter)
 - Component: Quad (1) (UnityEngine.MeshRenderer)
```
