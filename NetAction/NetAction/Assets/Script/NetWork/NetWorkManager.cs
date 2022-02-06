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
    static public NetWorkManager Incetance { get; private set; }

    private void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
            _roomsSubject.Subscribe(r => RoomListView.Instance.RoomListSetup(r));
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

  
    public int PlayerId;

    Thread thread;
    UdpClient client;

    Subject<Rooms.Room[]> _roomsSubject= new Subject<Rooms.Room[]>();

    public  enum SendMesageState
    {
        RogIn,
        RogOut,
        MetHod,
        NetWorkMetHod,
    }
    private void Start()
    {
        client = new UdpClient();
        client.Connect(host, port);

        var s = Scheduler.MainThread;//アクセスしておかないとMainThreadDispatcherが使えない

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

        if (client.Client.Connected)
        {
            GameManager.Instance.GamePreparation.OnNext(Unit.Default);
        }
       
    }
    void ThreadMetHod()
    {
        while (true)
        {
            IPEndPoint iP = null;
            byte[] date = client.Receive(ref iP);

            if (date == null) return;

            Receive(iP, date);
        }
    }
    void Receive(IPEndPoint iP, byte[] date)
    {
        var x = Encoding.UTF8.GetString(date);
            
        Dictionary<string,object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(x);

        if ((string)json["State"] == "PlayerId")
        {
            int id = int.Parse(json["PlayerID"].ToString());

            PlayerId = id;
        }
        if ((string)json["State"] == "RoomList")
        {
            var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));

            MainThreadDispatcher.Post(_ => _roomsSubject.OnNext(rooms.RoomList),null);
        }
        if ((string)json["State"] == "GameStart")
        {
            MainThreadDispatcher.Post(_ => GameManager.Instance.GameStart.OnNext(Unit.Default), null);
        }
    }

    public void SendJsonMessege(Messege messege)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(messege));

        client.Send(buffer, buffer.Length);
    }
    
    public struct Messege
    {
        public string Name;

        public SendMesageState State;

        public int PlayerId;

        public string Method;

        public Messege(string name, SendMesageState mesageState ,int playerId = 0 , string method = null)
        {
            Name = name;
            State = mesageState;
            PlayerId = playerId;
            Method = method;
        }
    }

    /// <summary>
    /// アプリケーション終了時
    /// </summary>
    void OnApplicationQuit()
    {
        var rogout = new Messege("RogOut", SendMesageState.RogOut,PlayerId);

        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(rogout));
        client.Send(buffer, buffer.Length);
        client.Close();
        thread.Abort();
    }
}
