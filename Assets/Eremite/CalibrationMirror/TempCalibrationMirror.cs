
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class TempCalibrationMirror : UdonSharpBehaviour
{

    [Tooltip("Mirror that will be toggled on.")]
    public GameObject mirror;
    [Tooltip("Button that toggles the mirror on (for disabling).")]
    public GameObject button;
    [Tooltip("Slider that shows countdown before mirror disables.")]
    public Slider timeSlider;
    [Tooltip("How long the mirror should stay active (seconds) before disbling itself.")]
    public float timeActive = 40;
    // local values
    private bool isActive = false;
    private float activateTime = 0;

    void Start()
    {
        mirror.SetActive(false);
        button.SetActive(true);
    }
    public void showMirror() {
        button.SetActive(false);
        mirror.SetActive(true);
        isActive=true;
        activateTime=Time.realtimeSinceStartup;
        timeSlider.value=0.0F;
    }
    void Update(){
        if ( isActive == true ) {
            float timeDiff = Time.realtimeSinceStartup - activateTime;
            if ( timeDiff >= timeActive ) {
                isActive = false;
                button.SetActive(true);
                mirror.SetActive(false);
                timeSlider.value = 1.0F;
            } else {
                timeSlider.value = 1.0f - (timeDiff / timeActive);
            }
        }
    }
}
