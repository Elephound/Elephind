using System.Collections.Generic;
using UnityEngine; // Use this if you're working in Unity
// using Newtonsoft.Json; // Use this if you're working outside Unity and using Json.NET

[System.Serializable]
public class GenericResponse
{
    public string chat_response;            // Corresponds to chat_response: string
    public ResponseCode response_code;      // Corresponds to response_code: "OK" | "ERROR" | "SEARCH_RESULT" | "CAPTURE_RESULT"
    public string input_text;               // Corresponds to input_text?: string (Optional)
    public string input_prompt;             // Corresponds to input_prompt?: string (Optional)
    public List<StorageUnit> storageunits;  // Corresponds to storageunits: StorageUnit[]
}

// Enum to represent the specific values for response_code
public enum ResponseCode
{
    OK,
    ERROR,
    SEARCH_RESULT,
    CAPTURE_RESULT
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
