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
        public string RoomNamber;

        public string RoomName;
    }
}

public class Room : MonoBehaviour
{
    public string RoomNamber;

    public string RoomName;

    public Text RoomText;

    public  void EnterRoom()
    {
        var enterRoom = new Dictionary<string, object>()
        {
           {"RoomName",RoomName },
           {"RoomNamber" ,RoomNamber},
        };

        var jsonEnterRoom = MiniJSON.Json.Serialize(enterRoom);
        var messege = new WebSocketManager.Messege("EnterRoom", WebSocketManager.Messege.MessegeState.Room, jsonEnterRoom);

        WebSocketManager.Incetance.WebSocketSendMessege(messege);

        GameManager.MyColor = GameManager.TrunpColor.Red;

        RoomListView.Incetance.InRoom(RoomName);
    }
}

