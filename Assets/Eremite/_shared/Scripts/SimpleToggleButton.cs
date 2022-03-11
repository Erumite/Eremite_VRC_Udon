using UdonToolkit;
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDKBase;
using VRC.Udon;

public class SimpleToggleButton : UdonSharpBehaviour
{
    // Set up the button mesh and on/off materials and whether it should display as on or off by default
    [Header("Button Options")]
    [Tooltip("The mesh renderer for the button.")]
    public MeshRenderer buttonMesh;
    [Tooltip("The material to display when 'Off'")]
    public Material offMaterial;
    [Tooltip("The material to display when 'On'")]
    public Material onMaterial;
    [Tooltip("Show the 'on' material by default - actual defaults are controlled below.")]
    public bool onMaterialDefault;
    private bool isOn;

    // A bunch of different types of things that can be toggles.
    //  Probably not too bad to loop through a bunch of empty arrays, but for max performance,
    //   this could be copied and all the unused stuff removed along with its toggle code below.

    [ListView("Game Objects")][LVHeader("Game Object")]
    public GameObject[] gameObjects;
    [ListView("Game Objects")][LVHeader("Default")]
    public bool[] gameObjectEnabled;

    [ListView("Mesh Renderers")][LVHeader("Mesh Renderer")]
    public MeshRenderer[] meshRenderers;
    [ListView("Mesh Renderers")][LVHeader("Default")]
    public bool[] meshRenderersEnabled;

    [ListView("Colliders")][LVHeader("Collider")]
    public Collider[] colliders;
    [ListView("Colliders")][LVHeader("Default")]
    public bool[] colliderEnabled;

    [ListView("Lights")][LVHeader("Light")]
    public Light[] lights;
    [ListView("Lights")][LVHeader("Default")]
    public bool[] lightsEnabled;

    [ListView("LookAt Constraints")][LVHeader("Constraint")]
    public LookAtConstraint[] lookatConstraints;
    [ListView("LookAt Constraints")][LVHeader("Default")]
    public bool[] lookatConstraintEnabled;

    async void Start()
    {
        // Set up the defaults at scene load based on user-inputted bools.
        for ( int i =0 ; i < gameObjects.Length ; i ++ ) {
            gameObjects[i].SetActive(gameObjectEnabled[i]);
        }
        for ( int m = 0 ; m < meshRenderers.Length ; m++ ) {
            meshRenderers[m].enabled = meshRenderersEnabled[m];
        }
        for ( int c = 0 ; c < colliders.Length ; c++ ) {
            colliders[c].enabled = colliderEnabled[c];
        }
        for ( int l = 0 ; l < lookatConstraints.Length ; l++ ) {
            lookatConstraints[l].constraintActive = lookatConstraintEnabled[l];
        }
        for ( int l = 0 ; l < lights.Length ; l++ ) { 
            lights[l].enabled = lightsEnabled[l];
        }
        // Update Material to On/Off based on user settings.
        isOn = onMaterialDefault;
        updateMaterial();
    }

    private void updateMaterial() {
        if (isOn) { 
            buttonMesh.material = onMaterial;
        } else {
            buttonMesh.material = offMaterial;
        }
    }

    public override async void Interact()
    {
        // Just set active to the opposite of whether it's active now.
        foreach ( GameObject i in gameObjects){
            i.SetActive(!i.activeInHierarchy);
        }
        foreach (MeshRenderer m in meshRenderers){
            m.enabled = !m.enabled;
        }
        foreach (Collider c in colliders){
            c.enabled = !c.enabled;
        }
        foreach (LookAtConstraint l in lookatConstraints) {
            l.constraintActive = !l.constraintActive;
        }
        foreach (Light l in lights){
            l.enabled = !l.enabled;
        }

        // Toggle button material.
        isOn = !isOn;
        updateMaterial();
    }
}
