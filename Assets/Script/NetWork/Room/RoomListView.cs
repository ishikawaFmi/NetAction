using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class RoomListView : MonoBehaviour
{
    public static RoomListView Incetance;

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

    [SerializeField]
    Button _exitButton;

    List<GameObject> _roomList = new List<GameObject>();

   public bool IsInRoom = false;
    private void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
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
            roomIns.RoomNamber = rooms[i].RoomNamber;
            roomIns.RoomText.text = roomIns.RoomName;

            _roomList.Add(roomIns.gameObject);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public  void CreateRoom()
    {
        var createRoom = new Dictionary<string, string>()
        {
           {"RoomName" ,_roomNameField.text},
        };

        var jsonCreateRoom = MiniJSON.Json.Serialize(createRoom);
        var messege = new WebSocketManager.Messege("CreateRoom", WebSocketManager.Messege.MessegeState.Room, jsonCreateRoom);

        WebSocketManager.Incetance.WebSocketSendMessege(messege);

        GameManager.MyColor = GameManager.TrunpColor.Black;

        InRoom(_roomNameField.text);
    }
    public void InRoom(string roomName)
    {      
        _roomListPanel.gameObject.SetActive(false);
        _roomNameText.gameObject.SetActive(true);

        IsInRoom = true;
        _roomNameText.text = $"現状の入室中のルーム{roomName} \n";

        _exitButton.gameObject.SetActive(true);
    }
    public void LeftRoom()
    {
        _roomListPanel.gameObject.SetActive(true);
        _roomNameText.gameObject.SetActive(false);
        _roomNameField.text = "";

        IsInRoom = false;

        _exitButton.gameObject.SetActive(false);
    }

    public void DeleteRoom()
    {
        if (GameManager.Incetance.WinCheak) return;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Result")
        {
            if (IsInRoom && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
                IsInRoom = false;
            }

        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Title" && IsInRoom)
        {
            LeftRoom();
        }
    }

    public void ExitRoom()
    {
        WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege(("ExitRoom"), WebSocketManager.Messege.MessegeState.Room));
        LeftRoom();
    }
}
