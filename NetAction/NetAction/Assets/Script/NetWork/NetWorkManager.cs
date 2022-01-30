using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Net;
using System.Text;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class NetWorkManager : MonoBehaviour
{
    static public NetWorkManager Incetance;

    private void Awake()
    {
        if (Incetance is null)
        {
            Incetance = this;
        }
        else
        {
            DontDestroyOnLoad(Incetance);
        }
    }
    [SerializeField]
    string host = "localHost";

    [SerializeField]
    int port = 50000;

    Thread thread;
    UdpClient client;

    Subject<Rooms.Room[]> _roomsSubject= new Subject<Rooms.Room[]>();

    public  enum SendMesageState
    {
        RogIn,
        RogOut,
        MetHod,
    }
    private void Start()
    {
        client = new UdpClient();
        client.Connect(host, port);
        if (!client.Client.Connected)
        {
            Debug.LogError("err");
        }
        else
        {
            Debug.Log("sec");
        }
        
        thread = new Thread(new ThreadStart(ThreadMetHod));
        thread.Start();
        var rogin = new Messege("Rogin", SendMesageState.RogIn);
       
        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(rogin));
        client.Send(buffer,buffer.Length);
   
        _roomsSubject.Subscribe(r => RoomListView.Instance.RoomListSetup(r));

        var s = Scheduler.MainThread;//アクセスしておかないとMainThreadDispatcherが使えない

    }
    void ThreadMetHod()
    {
        while (true)
        {
            IPEndPoint iP = null;
            byte[] date = client.Receive(ref iP);       
            Receive(iP, date);
        }
    }
    void Receive(IPEndPoint iP, byte[] date)
    {
        var x = Encoding.UTF8.GetString(date);
        Dictionary<string,object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(x);
        if ((string)json["State"] == "RoomList")
        {
            var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));

            MainThreadDispatcher.Post(_ => _roomsSubject.OnNext(rooms.RoomList),null);
        }
    }

    /// <summary>
    /// アプリケーション終了時
    /// </summary>
    void OnApplicationQuit()
    {
        client.Close();
        thread.Abort();
    }
    public struct Messege
    {
        public string Name;

        public SendMesageState State;
        public Messege(string name, SendMesageState mesageState)
        {
            Name = name;
            State = mesageState;
        }
    }
}
