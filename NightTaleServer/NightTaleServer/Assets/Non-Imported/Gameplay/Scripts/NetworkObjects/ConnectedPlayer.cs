using UnityEngine;
using Newtonsoft.Json;
using DarkRift.Server;
using System;
using System.Collections.Generic;

public class ConnectedPlayer
{

    [JsonIgnore]
    public string charID { get; set; }
    public PlayerPositionalData positionalData { get; set; } = null;
    public PlayerEquipData equipData { get; set; } = null;

    public static ConnectedPlayer GetPlayerDataFromJSON(string json, string charID) 
    {
        if (json == "null") 
        {
            return null;
        }
        var res = JsonConvert.DeserializeObject<ConnectedPlayer>(json);
        res.charID = charID;
        return res;
    }
    public string GetJsonString() 
    {
        return JsonConvert.SerializeObject(this);
    }
}
[JsonObject]
public class PlayerPositionalData
{
    public ushort templateID { get; set; }
    public uint instanceID { get; set; }
    [JsonIgnore]
    public Vector3 position { get => new Vector3(posX,posY,posZ); set { posX = value.x; posY = value.y; posZ = value.z; } }
    [JsonProperty]
    public float posX;
    [JsonProperty]
    public float posY;
    [JsonProperty]
    public float posZ;
}
[JsonObject]
public class PlayerEquipData 
{
    public List<EquipItem> equippedItems = new List<EquipItem>();
    public int primaryHandSkillID { get; set; }
    public int secondaryHandSkillID { get; set; }
}
[JsonObject]
public struct EquipItem 
{
    public int slotID { get; set; }
    public int itemID { get; set; }
}
