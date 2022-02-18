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
        public int RoomID;

        public string RoomName;

        public int MaxRoomMenber;

    }
}

public class Room : MonoBehaviour
{
    public int RoomID;

    public string RoomName;

    public int MaxRoomMenber;

    public Text RoomText;

    public async void EnterRoom()
    {
        var enterRoom = new Dictionary<string, object>()
        {
           {"RoomName",RoomName },
           {"RoomID" ,RoomID},
        };

        var jsonEnterRoom = MiniJSON.Json.Serialize(enterRoom);
        var messege = new NetWorkManager.Messege("EnterRoom", NetWorkManager.SendMesageState.NetWorkMetHod, NetWorkManager.Incetance.PlayerId, jsonEnterRoom);

        await NetWorkManager.Incetance.SendJsonMessege(messege);


        RoomListView.Instance.InRoom(RoomName);
    }
}

