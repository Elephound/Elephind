using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;



[System.Serializable]
public class Room
{
    public int RoomID;
    public List<StorageContainer> StorageContainers;

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
        ScreenshotData = texture2D.EncodeToJPG();
        ContainerID = containerID;
    }

     public StorageContainer(int containerID, Texture2D texture2D, string description)
     {
        Description = description;
        ScreenshotData = texture2D.EncodeToJPG();
        ContainerID = containerID;
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
    public Room room;

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
        LoadRoomData();

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

    public bool UpdateContainerScreenshot()
    {
        if(activeContainer == null)
        return false;

        activeContainer.GetInstanceID();
        //try to find container in list, if present update screenshot, if not create
        return true;
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
            return;
        }

        if(activeContainer == containerMono)
            activeContainer = null;
        else
            activeContainer = containerMono;

    }




}
