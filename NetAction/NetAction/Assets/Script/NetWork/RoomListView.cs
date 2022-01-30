using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListView : MonoBehaviour
{
    public static RoomListView Instance;

    [SerializeField]
    GameObject _roomListView = null;

    [SerializeField]
    GameObject _roomPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public  void RoomListSetup(Rooms.Room[] rooms)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            var room = Instantiate(_roomPrefab, _roomListView.transform).GetComponent<Room>();
            room.RoomName = rooms[i].RoomName;
            room.MaxRoomMenber = rooms[i].MaxRoomMenber;
        }
    }
}
