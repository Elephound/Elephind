using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;



[System.Serializable]
public class Room
{
    public int RoomID;
    public List<StorageContainer> StorageContainers;

    public Room()
    {
        StorageContainers = new List<StorageContainer>();
    }

    public void AddStorageContainer(StorageContainer storageContainer)
    {
        StorageContainers.Add(storageContainer);
    }

    public void RemoveStorageContainer(StorageContainer storageContainer)
    {
        StorageContainers.Remove(storageContainer);
    }
}


public class StorageContainer
{
    public int ContainerID;
    public byte[] ScreenshotData;
    public string Description;

    public StorageContainer(int containerID, Texture2D texture2D)
    {
        Description = "";
       if(texture2D!=null) 
            ScreenshotData = texture2D.EncodeToJPG();
            else
            ScreenshotData= null;
        ContainerID = containerID;
    }

     public StorageContainer(int containerID, Texture2D texture2D, string description)
     {
        Description = description;
        ScreenshotData = texture2D.EncodeToJPG();
        ContainerID = containerID;
     }

     public void UpdateTextureData(Texture2D texture2D)
     {
            ScreenshotData = texture2D.EncodeToJPG();
     }

    public Texture2D GetTexture2D()
    {
         Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(ScreenshotData); // Load texture from byte array
        return texture;
    }
}


public class StorageContainerManager : MonoBehaviour
{

    int _roomID;
    List<StorageContainer> _storageContainers = new List<StorageContainer>();
    public Room room = new Room();

    private string _filePath;
    private string _folderPath = "/Rooms";

    static public StorageContainerManager Instance;

    StorageContainerMono activeContainer;

    public bool IsInSetupPhase=false;

    public UnityEvent<bool> SetupPhaseChanged;


    void Awake()    
    {
        if(Instance == null)
            Instance = this;
    }


    void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, _folderPath);
        room = LoadRoomData();


    }

    public void ToggleSetupPhase()
    {
        if(IsInSetupPhase)
            EndSetupPhase();
        else
            StartSetupPhase();
    }

    public void StartSetupPhase()
    {
        SetupPhaseChanged.Invoke(true);
        IsInSetupPhase=true;
    }

    public void EndSetupPhase()
    {
        SetupPhaseChanged.Invoke(false);
        IsInSetupPhase=false;
    }

    public void CreateStorageContainer()
    {
        StorageContainer newStorageContainer = new StorageContainer(activeContainer.GetContainerID(), null);
        room.AddStorageContainer(newStorageContainer);
    }


    public bool UpdateContainerScreenshot(Texture2D texture)
    {
        if(activeContainer == null)
        {
            Debug.LogError("no active Container");
            return false;
        }

        Debug.LogWarning("Updating Screenshot");
        int id = activeContainer.GetInstanceID();
        StorageContainer storageToUpdate = room.StorageContainers.Find(storage => storage.ContainerID == id);
        storageToUpdate.UpdateTextureData(texture);

        SendToBackend(storageToUpdate);
        return true;
    }

    void SendToBackend(StorageContainer storageToUpdate)
    {
        WebRequester.Instance.SendStorageUnit(storageToUpdate);
    }

    public StorageContainer GetStorageContainerData(int containerID)
    {
        if(room.StorageContainers == null)
        {
            Debug.LogError("Storage Containers null");
            return null;
        }

        return room.StorageContainers.Find(storage => storage.ContainerID == containerID);
    }


    public void SaveRoomData(Room roomData)
    {
        string jsonData = JsonUtility.ToJson(roomData);
        string finalFilePath = Path.Combine(Application.persistentDataPath, roomData.RoomID + ".json");
        File.WriteAllText(finalFilePath, jsonData);
        Debug.Log("Data saved to " + _filePath);
    }

    public Room LoadRoomData()
    {
          string finalFilePath = Path.Combine(Application.persistentDataPath, _roomID + ".json");
        if (File.Exists(_filePath))
        {
            string jsonData = File.ReadAllText(_filePath);
            Room roomData = JsonUtility.FromJson<Room>(jsonData);
            Debug.Log("Data loaded from " + finalFilePath);
            return roomData;
        }
        else
        {
            Debug.LogWarning("No data found at " + finalFilePath);
            return new Room(); // Return an empty data object if no file is found
        }
    }

    public void SetContainerAsActive(StorageContainerMono containerMono)
    {
        if(activeContainer == null)
        {
            activeContainer = containerMono;
            containerMono.SetContainerActiveVisual(true);
            return;
        }

        activeContainer.SetContainerActiveVisual(false);

        if(activeContainer == containerMono)
            activeContainer = null;
        else
            activeContainer = containerMono;

        containerMono.SetContainerActiveVisual(true);

    }




}
