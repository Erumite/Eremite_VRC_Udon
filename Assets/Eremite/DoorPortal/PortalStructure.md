# Anatomy of a Portal
This is what a portal looks like in game after the client generates the internal game objects, assuming the root object is named "Portal".

Generated using [this UdonBehaviour](/Assets/Eremite/Keypad-3D)

These can be interacted with via Udon# or Animators.

```
/Portal
 - Component: Portal (UnityEngine.Transform)
 - Component: Portal (VRC.SDK3.Components.VRCPortalMarker)
 - Component: Portal (VRC.Udon.UdonBehaviour)
 - Component: View 12 on Portal

/Portal/PortalInternal(Clone)
 - Component: PortalInternal(Clone) (UnityEngine.Transform)
 - Component: PortalInternal(Clone) (UnityEngine.BoxCollider)

/Portal/PortalInternal(Clone)/PortalGraphics
 - Component: PortalGraphics (UnityEngine.Transform)

/Portal/PortalInternal(Clone)/PortalGraphics/PortalCore
 - Component: PortalCore (UnityEngine.Transform)
 - Component: PortalCore (UnityEngine.MeshFilter)
 - Component: PortalCore (UnityEngine.MeshRenderer)

/Portal/PortalInternal(Clone)/PortalGraphics/PortalFringe
 - Component: PortalFringe (UnityEngine.Transform)
 - Component: PortalFringe (UnityEngine.ParticleSystem)
 - Component: PortalFringe (UnityEngine.ParticleSystemRenderer)

/Portal/PortalInternal(Clone)/NameTag
 - Component: NameTag (UnityEngine.Transform)

/Portal/PortalInternal(Clone)/NameTag/TextMesh
 - Component: TextMesh (UnityEngine.RectTransform)
 - Component: TextMesh (UnityEngine.MeshRenderer)
 - Component: TextMesh (UnityEngine.CanvasRenderer)
 - Component: TextMesh (TMPro.TextMeshPro)
 - Component: TextMesh (UnityEngine.MeshFilter)

/Portal/PortalInternal(Clone)/PlatformIcons
 - Component: PlatformIcons (UnityEngine.Transform)

/Portal/PortalInternal(Clone)/PlatformIcons/Quad
 - Component: Quad (UnityEngine.Transform)
 - Component: Quad (UnityEngine.MeshFilter)
 - Component: Quad (UnityEngine.MeshRenderer)

/Portal/PortalInternal(Clone)/PlatformIcons/Quad (1)
 - Component: Quad (1) (UnityEngine.Transform)
 - Component: Quad (1) (UnityEngine.MeshFilter)
 - Component: Quad (1) (UnityEngine.MeshRenderer
```
