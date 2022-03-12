using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocketSharp.Server;


public class WebSocketManager : MonoBehaviour
{
    static public WebSocketManager Incetance { get; private set; }

    [SerializeField]
    string _ipAddres = "13.231.242.11";

    [SerializeField]
    int _port = 5001;

    [SerializeField]
     string _id;

    [SerializeField]
    Text text = null;

    [DllImport("__Internal")]
    private static extern void Connect(string server);

    [DllImport("__Internal")]
    private static extern string GetMessage();

    [DllImport("_Internal")]
    private static extern void SendMessage(Messege messege);

    private void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
            Connect($"ws://{_ipAddres}:{_port}");
            StartCoroutine(ReciveMessege());
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //void Connect()
    //{
    //    var s = Scheduler.MainThread;//宣言しておかないとMainThread

    //    if (WebSocket == null)
    //    {
            
    //           WebSocket = new WebSocket("ws://13.231.242.11:5001");

    //        WebSocket.OnMessage += (sender, e) =>
    //        {
    //            Sort(e.Data);
    //        };

    //        WebSocket.ConnectAsync();

    //        if (WebSocket != null)
    //        {
             
    //            Debug.Log("sec");
    //        }
    //        else
    //        {
    //            text.text = "not";
    //            Debug.LogError("notConect");
    //        }

          
    //    }
    //}
   
    IEnumerator ReciveMessege()
    {
        while (true)
        {
            string json = GetMessage();

            if (json == null)
            {
                Sort(json);
            }

            yield return null;
        }
    }

  

    void Sort(string data)
    {
        Dictionary<string, object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(data);


        switch (json["State"])
        {
            case "Rogin":
                _id = (string)json["Id"];
                Debug.Log(_id);
                break;
            case "RoomList":
                var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));
                MainThreadDispatcher.Post(_ => RoomListView.Incetance.RoomListSetup(rooms.RoomList), null);
                break;
            default:
                break;
        }
    }

    public void SendMessege(Messege messge)
    {
        SendMessage(messge);
    } 

  public struct Messege
    {
        public MessegeState State;

        public string MethodName;

        public string MethodData;
        public enum MessegeState
        {
            Room,

        }

        public Messege(string methodName, MessegeState messegeState, string methodData = null)
        {
            this.State = messegeState;
            this.MethodName = methodName;
            this.MethodData = methodData;
        }

    }
}
