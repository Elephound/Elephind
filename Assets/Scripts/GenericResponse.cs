using System.Collections.Generic;
using UnityEngine; // Use this if you're working in Unity
// using Newtonsoft.Json; // Use this if you're working outside Unity and using Json.NET

[System.Serializable]
public class GenericResponse
{
    public string chat_response;
    public string response_code; // storageunit_added, itemsearch_finished
    public string input_text; // Optional field
    public string input_prompt; // Optional field
    public List<StorageUnit> storageunits;
}

[System.Serializable]
public class StorageUnit
{
    public string id;                       // Required
    public string name;                     // Optional
    public string description;              // Optional
    public List<Item> items;                // Required: List of items
}

[System.Serializable]
public class Item
{
    public string name;                     // Required
    public string description;              // Optional
    public string ean;                      // Optional
    public string category;                 // Optional
    public int? quantity;                   // Optional: Nullable int
}
