using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StorageContainerMono : MonoBehaviour
{

    //to be set when creating container
    [SerializeField] int _containerID;
    [SerializeField] Renderer _renderer;
    [SerializeField] Material _normalMaterial;
    [SerializeField] Material _activeMaterial;

    public void CreateContainer(int containerID)
    {
        _containerID = containerID;
    }

    public void SetContainerAsActive()
    {
        StorageContainerManager.Instance.SetContainerAsActive(this);
    }

    public void SetContainerActiveVisual(bool value)
    {
        if (value == true)
            _renderer.material = _activeMaterial;
        else
            _renderer.material = _normalMaterial;
    }



}


