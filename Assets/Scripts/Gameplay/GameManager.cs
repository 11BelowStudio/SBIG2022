#nullable enable
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scripts.Utils.Extensions.ListExt;
using UnityEngine;
using Scripts.Utils.Types;
using Scripts.Utils.Annotations;
namespace Scripts.Gameplay
{
    public class GameManager: Singleton<GameManager>
    {

        

        private Dictionary<CameraEnum, RoomStruct> _rooms = new Dictionary<CameraEnum, RoomStruct>();

        [SerializeField] [ReadOnly] private List<RoomStruct> _roomsList = new List<RoomStruct>();
        
        public IReadOnlyDictionary<CameraEnum, RoomStruct> Rooms => _rooms;


        public event Action<EnemyEnum, EnemyPositionStruct> OnEnemyMoved;


        private void Awake()
        {
            if (!_AttemptToRegisterInstance)
            {
                Destroy(this);
                return;
            }
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            // bruh;
        }
        
        private void OnValidate(){
            ReloadRoomStructs();
        }

        private void ReloadRoomStructs()
        {
            List<EnemyLocationNode> allEnemyLocations = new List<EnemyLocationNode>(FindObjectsOfType<EnemyLocationNode>());
            List<RoomCamera> allRoomCameras = new List<RoomCamera>(FindObjectsOfType<RoomCamera>());
            _rooms.Clear();
            _roomsList.Clear();
            foreach (CameraEnum currentCam in Enum.GetValues(typeof(CameraEnum)))
            {
                var thisRoom = new RoomStruct
                {
                    theRoom = currentCam,
                    theRoomCamera = allRoomCameras.FindButDontThrowIfNull(rc => rc.IsThisMyCam(currentCam)),
                    nodesInThisRoom = allEnemyLocations.FindAll(en => en.IsThisMyCam(currentCam))
                };
                _rooms.Add(
                    currentCam,
                    thisRoom
                );
                _roomsList.Add(thisRoom);
                
            }
            
            
        }
    }


    
    
}