
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InteractLocalToggleGameObject : UdonSharpBehaviour
{
    public GameObject[] gameObjects;
    void Start(){}
    public override void Interact()
    {
        for (int i=0; i<gameObjects.Length ; i++ ) {
            gameObjects[i].SetActive(!gameObjects[i].activeInHierarchy);
        }
    }
}
