using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]

public class Rooms
{
    public Room[] RoomList;

    [Serializable]
    public class Room
    {
        public string RooomNamber;

        public string RoomName;

    }
}

public class Room : MonoBehaviour
{
    public string RooomNamber;

    public string RoomName;

    public Text RoomText;

    public  void EnterRoom()
    {
        var enterRoom = new Dictionary<string, object>()
        {
           {"RoomName",RoomName },
           {"RoomID" ,RooomNamber},
        };

        var jsonEnterRoom = MiniJSON.Json.Serialize(enterRoom);
    //    var messege = new WebSocketManager.Messege();

      


        RoomListView.Incetance.InRoom(RoomName);
    }
}

