using UnityEngine;
using Newtonsoft.Json;
using DarkRift.Server;

public class PlayerData
{
    [JsonIgnore]
    public string charID { get; set; }
    public PlayerPositionalData positionalData { get; set; }



    public static PlayerData GetPlayerDataFromJSON(string json, string charID) 
    {
        if (json == "null") 
        {
            return null;
        }
        var res = JsonConvert.DeserializeObject<PlayerData>(json);
        res.charID = charID;
        return res;
    }
    public string GetJsonString() 
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class PlayerPositionalData
{
    public ushort templateID { get; set; }
    public uint instanceID { get; set; }
    public Vector3 position { get; set; }
}
