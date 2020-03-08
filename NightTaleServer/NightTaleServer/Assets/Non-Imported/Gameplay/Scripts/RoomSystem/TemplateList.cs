using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.Server.RoomManagement
{
    [CreateAssetMenu(menuName = "FYP/TemplateList")]
    public class TemplateList : ScriptableObject
    {
        [ReorderableList]
        [SerializeField]
        private List<RoomTemplate> roomTemplates = new List<RoomTemplate>();

        public int GetRoomID(RoomTemplate roomTemplate) 
        {
            return roomTemplates.IndexOf(roomTemplate);
        }
        public RoomTemplate GetTemplateID(int index) 
        {
            if(index >= 0 && index < roomTemplates.Count) 
            {
                return roomTemplates[index];
            }
            return null;
        }
    }
}