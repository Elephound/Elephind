using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.CallbackHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;


public class StorageContainerMono : MonoBehaviour
{

    //to be set when creating container
    private OVRSpatialAnchor _ownSpatialAnchor;


    private int _containerID;

    [SerializeField] Renderer _renderer;
    [SerializeField] Material _normalMaterial;
    [SerializeField] Material _activeMaterial;
    [SerializeField] Collider _ownCollider;


    bool firsStageActive = true;
    bool secondStageActive = true;
    [SerializeField] GameObject _firstStageUI;
    [SerializeField] TextMeshProUGUI _containerName;
     [SerializeField] TextMeshProUGUI _contentText;
    [SerializeField] GameObject _secondStageUI;
      [SerializeField] Image _containerImage;

    public bool IsActivated;

    void Awake()
    {
        _ownSpatialAnchor = this.transform.GetComponentInParent<OVRSpatialAnchor>();
        if(_ownSpatialAnchor == null) 
            Debug.LogError("spawned but didnt find own anchor");
        _containerID = GuidToInt(_ownSpatialAnchor.Uuid);

        StorageContainerManager.Instance.SetContainerAsActive(this);
        StorageContainerManager.Instance.CreateStorageContainer();

    }

    static int GuidToInt(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        // Take the first 4 bytes to create an integer
        int result = BitConverter.ToInt32(bytes, 0);
        return result;
    }


    void Start()
    {
        SetFirstStageUIActive(false);
        SetSecondStageUIActive(false);


        UpdateUIContent();
    }


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

    void UpdateUIContent()
    {
        StorageContainer containerData = StorageContainerManager.Instance.GetStorageContainerData(_containerID);
        if(containerData== null)
            return;
        //_contentText.text = containerData.items;
        _containerName.text = containerData.Description;
        Texture2D texture = containerData.GetTexture2D();
        if(texture != null)   
        { 
            _containerImage.sprite = ConvertTexture2DToSprite(texture);
        }
        else
            _containerImage.gameObject.SetActive(false);

    }

    public void SetFirstStageUIActive(bool value)
    {
        if(firsStageActive == value)
            return;
        if(value)
            UpdateUIContent();

        _firstStageUI.SetActive(value);
         firsStageActive =value;

    }

    public void SetSecondStageUIActive(bool value)
    {
         if(secondStageActive == value)
            return;

          if(value)
            UpdateUIContent();

        _secondStageUI.SetActive(value);
        secondStageActive =value;
    }

    private Sprite ConvertTexture2DToSprite(Texture2D texture)
    {
        // Create a new Sprite from the Texture2D
        return Sprite.Create(
            texture, 
            new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f), 
            100.0f); // Pixels per unit, adjust based on your requirements
    }

    
    



}


