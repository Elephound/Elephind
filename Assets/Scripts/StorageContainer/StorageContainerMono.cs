using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.CallbackHandlers;
using Unity.VisualScripting;
using UnityEngine;


public class StorageContainerMono : MonoBehaviour
{

    //to be set when creating container
    [SerializeField] int _containerID;
    [SerializeField] Renderer _renderer;
    [SerializeField] Material _normalMaterial;
    [SerializeField] Material _activeMaterial;
    [SerializeField] Collider _ownCollider;

    public bool IsActivated;

    void OnEnable()
    {
        StorageContainerManager.Instance.SetupPhaseChanged.AddListener(ToggleCollider);
    }

      void OnDisable()
    {
        StorageContainerManager.Instance.SetupPhaseChanged.RemoveListener(ToggleCollider);
    }

    void ToggleCollider(bool value)
    {
        _ownCollider.gameObject.SetActive(value);
    }
    public void CreateContainer(int containerID)
    {
        _containerID = containerID;
    }

    public void SetContainerAsActive(bool value)
    {
        if(value == true)
            StorageContainerManager.Instance.SetContainerAsActive(this);
        IsActivated = value;
        SetContainerActiveVisual(value);
    }

    public void SetContainerActiveVisual(bool value)
    {
        if (value == true)
            _renderer.material = _activeMaterial;
        else
            _renderer.material = _normalMaterial;
    }

    public int GetContainerID()
    {
        return _containerID;
    }



}


