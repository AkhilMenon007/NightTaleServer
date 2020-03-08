using UnityEngine;
using NaughtyAttributes;

namespace FYP.Server.RoomManagement
{
    [CreateAssetMenu(menuName = "FYP/RoomTemplate")]
    public class RoomTemplate : ScriptableObject
    {
        [SerializeField]
        private TemplateList templateList = null;
        [SerializeField]
        private Room roomPrefab = null;

        [SerializeField]
        private bool _global = true;

        [SerializeField]
        private RoomTemplate _fallbackRoom = null;

        [SerializeField]
        private int _serverSceneID = 0;

        [SerializeField]
        [Tooltip("0 for no limit")]
        private ushort _playerLimit = 0;


        [BoxGroup("Locality of Relevance")]
        [SerializeField]
        private int _lorCountX = 1;
        [BoxGroup("Locality of Relevance")]
        [SerializeField]
        private int _lorCountY = 1;
        [BoxGroup("Locality of Relevance")]
        [SerializeField]
        private int _lorCountZ = 1;
        [BoxGroup("Locality of Relevance")]
        [SerializeField]
        private Vector3 _padding = Vector3.one;
        [BoxGroup("Locality of Relevance")]
        [SerializeField]
        private Vector3 _sceneWidth = new Vector3(100f, 100f, 100f);
        [BoxGroup("Locality of Relevance")]
        [SerializeField]
        private Vector3 _centre = Vector3.zero;

        public bool isGlobal => _global;
        public RoomTemplate fallbackRoom => _fallbackRoom;
        public int lorCountX => _lorCountX;
        public int lorCountY => _lorCountY;
        public int lorCountZ => _lorCountZ;
        public Vector3 sceneSize => _sceneWidth;
        public Vector3 centre => _centre;
        public Vector3 padding => _padding;
        public ushort playerLimit => _playerLimit;
        public int serverSceneID => _serverSceneID;


        public ushort templateID => (ushort)templateList.GetRoomID(this);

        private void OnValidate()
        {
            _lorCountX = lorCountX > 0 ? lorCountX : 1;
            _lorCountY = lorCountY > 0 ? lorCountY : 1;
            _lorCountZ = lorCountZ > 0 ? lorCountZ : 1;

            _sceneWidth.x = _sceneWidth.x > 0f ? _sceneWidth.x : 1f;
            _sceneWidth.y = _sceneWidth.y > 0f ? _sceneWidth.y : 1f;
            _sceneWidth.z = _sceneWidth.z > 0f ? _sceneWidth.z : 1f;

            _playerLimit = _playerLimit >= 0 ? _playerLimit : (ushort)0;
        }

        public Room CreateRoom(uint id) 
        {
            var room = Instantiate(roomPrefab);
            room.Initialize(this,id);
            return room;
        }
    }
}