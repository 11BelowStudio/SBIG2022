#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Gameplay
{
    [Serializable]
    public struct RoomStruct: IHaveACameraEnum
    {
        [SerializeField]
        public CameraEnum theRoom;

        public RoomCamera? theRoomCamera;

        [SerializeField]
        public List<EnemyLocationNode> nodesInThisRoom;

        public bool TryGetRoomCam(out RoomCamera roomCam)
        {
            roomCam = theRoomCamera;
            return !(theRoomCamera is null);
        }

        public CameraEnum CamEnum => theRoom;
    }
}