using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.CallbackHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UIElements;


public class StorageContainerMono : MonoBehaviour
{

    //to be set when creating container
    private OVRSpatialAnchor _ownSpatialAnchor;


    private int _containerID;

    [SerializeField] Renderer _renderer;
    [SerializeField] Material _normalMaterial;
    [SerializeField] Material _activeMaterial;
    [SerializeField] Material _markedMaterial;
     [SerializeField] Material _searchMaterial;
    [SerializeField] Collider _ownCollider;


    bool firsStageActive = true;
    bool secondStageActive = true;

      [SerializeField] GameObject _UIParent;
    [SerializeField] GameObject _firstStageUI;
    [SerializeField] TextMeshProUGUI _containerNameStage1;
    [SerializeField] TextMeshProUGUI _containerNameStage2;
    [SerializeField] TextMeshProUGUI _contentText;
    [SerializeField] GameObject _secondStageUI;
    [SerializeField] UnityEngine.UI.Image _containerImage;

    public bool IsActivated;

    void Awake()
    {
        _ownSpatialAnchor = this.transform.GetComponentInParent<OVRSpatialAnchor>();
        if (_ownSpatialAnchor == null)
            Debug.LogError("spawned but didnt find own anchor");
        _containerID = GuidToInt(_ownSpatialAnchor.Uuid);

        StorageContainerManager.Instance.CreateStorageContainer(_containerID);
        GazePointer.Instance.RegisterSelf(this);

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
        StorageContainerManager.Instance.RegisterStorageContainerMono(this);

    }

    void OnDisable()
    {
        StorageContainerManager.Instance.SetupPhaseChanged.RemoveListener(ToggleCollider);
    }




    void ToggleCollider(bool value)
    {
        _ownCollider.enabled = value;
    }
    public void CreateContainer(int containerID)
    {
        _containerID = containerID;
    }

    public void SetContainerAsActive(bool value)
    {
        if (value == true)
            StorageContainerManager.Instance.SetContainerAsActive(this);
        IsActivated = value;
    }

    public void SetContainerActiveVisual(int value)
    {
        if (value == 0)
            _renderer.material = _normalMaterial;
        else if (value == 1)
            _renderer.material = _activeMaterial;
        else if (value == 2)
            _renderer.material = _markedMaterial;
        else if (value == 3)
            _renderer.material = _searchMaterial;
    }



    public int GetContainerID()
    {
        return _containerID;
    }

    void UpdateUIContent()
    {
        StorageContainer containerData = StorageContainerManager.Instance.GetStorageContainerData(_containerID);
        if (containerData == null)
            return;

        if(containerData.Description == String.Empty)
            return;
        _containerNameStage1.text = containerData.Description;
        _containerNameStage2.text = containerData.Description;

        //_contentText.text = containerData.items;

      /*  Texture2D texture = containerData.GetTexture2D();
        if (texture != null)
        {
            _containerImage.sprite = ConvertTexture2DToSprite(texture);
        }
        else
            _containerImage.gameObject.SetActive(false);
            */

    }

    public void SetUIActive(bool value)
    {
        _UIParent.SetActive(value);
    }

    public void SetFirstStageUIActive(bool value)
    {
        if (firsStageActive == value)
            return;
        if (value)
            UpdateUIContent();

        _firstStageUI.SetActive(value);
        firsStageActive = value;

    }

    public void SetSecondStageUIActive(bool value)
    {
        if (secondStageActive == value)
            return;

        if (value)
            UpdateUIContent();

        _secondStageUI.SetActive(value);
        secondStageActive = value;
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


