using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]

public class Rooms
{
    public Room[] RoomList;

    [Serializable]
    public class Room
    {
        public string RoomName;

        public int MaxRoomMenber;

    }
    
}
public class Room : MonoBehaviour
{
    public string RoomName;

    public int MaxRoomMenber;
}
