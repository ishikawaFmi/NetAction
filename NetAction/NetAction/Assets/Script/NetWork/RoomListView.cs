using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class RoomListView : MonoBehaviour
{
    public static RoomListView Instance;

    [SerializeField]
    GameObject _roomListView;

    [SerializeField]
    GameObject _roomListPanel;

    [SerializeField]
    GameObject _roomPrefab;

    [SerializeField]
    InputField _roomNameField;

    [SerializeField]
    Text _roomNameText;

    List<GameObject> _roomList = new List<GameObject>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public  void RoomListSetup(Rooms.Room[] rooms)
    {
        if (rooms == null) return;

        foreach (var room in _roomList)
        {
           Destroy( room.gameObject);
        }
        for (int i = 0; i < rooms.Length; i++)
        {
            
            var roomIns = Instantiate(_roomPrefab, _roomListView.transform).GetComponent<Room>();

            roomIns.RoomName = rooms[i].RoomName;
            roomIns.MaxRoomMenber = rooms[i].MaxRoomMenber;
            roomIns.RoomID = rooms[i].RoomID;
            roomIns.RoomText.text = roomIns.RoomName;

            _roomList.Add(roomIns.gameObject);
        }
    }
    /// <summary>
    /// 新しくルームを作成する
    /// </summary>
    public void CreateRoom()
    {
        var createRoom = new Dictionary<string, string>()
        {
           {"RoomName" ,_roomNameField.text},
        };

        var jsonCreateRoom = MiniJSON.Json.Serialize(createRoom);
        var messege = new NetWorkManager.Messege("CreateRoom",NetWorkManager.SendMesageState.NetWorkMetHod,NetWorkManager.Incetance.PlayerId, jsonCreateRoom);

        NetWorkManager.Incetance.SendJsonMessege(messege);

        InRoom(_roomNameField.text);
    }
    public void InRoom(string roomName)
    {      
        _roomListPanel.gameObject.SetActive(false);
        _roomNameText.gameObject.SetActive(true);

        _roomNameText.text = $"現在のルーム名　:　{roomName} \n マッチング中 ";
    }
}
