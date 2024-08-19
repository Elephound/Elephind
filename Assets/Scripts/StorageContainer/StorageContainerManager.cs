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

    public List<Item> Items;

    public StorageContainer(int containerID, Texture2D texture2D)
    {
        Description = "";
       if(texture2D!=null) 
            ScreenshotData = texture2D.EncodeToJPG();
            else
            ScreenshotData= null;
        ContainerID = containerID;

        Items = new List<Item>();
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

     public void AddItemToList(Item item)
     {
        Items.Add(item);
     }

    public Texture2D GetTexture2D()
    {
         Texture2D texture = new Texture2D(1440, 1440);
        texture.LoadImage(ScreenshotData); // Load texture from byte array
        return texture;
    }
}


public class StorageContainerManager : MonoBehaviour
{

    int _roomID;
    List<StorageContainerMono> _storageContainerMonos = new List<StorageContainerMono>();
    public Room room = new Room();

    private string _filePath;
    private string _folderPath = "/Rooms";

    static public StorageContainerManager Instance;

    StorageContainerMono activeContainer;

    public bool IsInSetupPhase=false;

    public UnityEvent<bool> SetupPhaseChanged;

    [SerializeField] GameObject DEBUGOBJECT;


    void Awake()    
    {
        if(Instance == null)
            Instance = this;
    }


    void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, _folderPath);
        room = new Room(); //LoadRoomData();

        DEBUGOBJECT.SetActive(false);

    }

    public void UpdateContainerData(int id, string description, List<Item> items)
    {
        StorageContainer storageContainer = room.StorageContainers.Find(storage => storage.ContainerID == id);
        storageContainer.Description = description;
        storageContainer.Items = items;
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

        DEBUGOBJECT.SetActive(true);
        DisableAllHighlights();
    }

    public void EndSetupPhase()
    {
        SetupPhaseChanged.Invoke(false);
        IsInSetupPhase=false;

        DEBUGOBJECT.SetActive(false);
        activeContainer = null;
        DisableAllHighlights();

    }

    public void RegisterStorageContainerMono(StorageContainerMono storageContainerMono )
    {
        _storageContainerMonos.Add(storageContainerMono);
    }

    void DisableAllHighlights()
    {
        foreach(StorageContainerMono storageContainerMono in _storageContainerMonos)
        {
            storageContainerMono.SetContainerActiveVisual(false);
        }
    }

    public void HighlighSpecificContainer(List<int> ids)
    {
        foreach(int id in ids)
        {
            foreach(StorageContainerMono storageContainerMono in _storageContainerMonos)
            {
                if(storageContainerMono.GetContainerID() == id)
                    storageContainerMono.SetContainerActiveVisual(true);
            }
        }
    }

    public void CreateStorageContainer(int containerID)
    {
        StorageContainer newStorageContainer = new StorageContainer(containerID, null);
        room.AddStorageContainer(newStorageContainer);
    }


    public int UpdateContainerScreenshot(Texture2D texture)
    {
        if(activeContainer == null)
        {
            Debug.LogError("no active Container");
            return -1;
        }

        int id = activeContainer.GetContainerID();

        StorageContainer storageToUpdate = room.StorageContainers.Find(storage => storage.ContainerID == id);
        if(storageToUpdate == null)
        {Debug.LogError("StorageToUpdate is null");
        return -1;
        }
        storageToUpdate.UpdateTextureData(texture);

        SendToBackend(storageToUpdate);
        return id;
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
