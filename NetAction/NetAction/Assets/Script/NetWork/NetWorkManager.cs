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
using System;

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
    public UdpClient Client;

    Subject<Rooms.Room[]> _roomsSubject = new Subject<Rooms.Room[]>();

    public enum SendMesageState
    {
        RogIn,
        RogOut,
        MetHod,
        NetWorkMetHod,
    }
    private void Start()
    {
        Client = new UdpClient();
        Client.Connect(host, port);

        var s = Scheduler.MainThread;//アクセスしておかないとMainThreadDispatcherが使えない

        if (!Client.Client.Connected)
        {
            Debug.LogError("err");
        }
        else
        {
            Debug.Log("sec");
        }

        thread = new Thread(new ThreadStart(ThreadMetHod));

        thread.Start();

        if (Client.Client.Connected)
        {
            GameManager.Instance.GamePreparation.OnNext(Unit.Default);
        }

    }
    /// <summary>
    /// クライアントからデータを受信する
    /// </summary>
    void ThreadMetHod()
    {
        while (true)
        {
            IPEndPoint iP = null;
            byte[] data = Client.Receive(ref iP);

            if (data == null) return;

            Receive(data);
        }
    }
    /// <summary>
    /// 受信したデータをの処理をする
    /// </summary>
    /// <param name="data"></param>
    void Receive(byte[] data)
    {
        var x = Encoding.UTF8.GetString(data);

        Dictionary<string, object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(x);//送られてきたデータをDictionaryに変換する

        switch ((string)json["State"])
        {
            case "PlayerId":
                int id = int.Parse(json["PlayerID"].ToString());

                PlayerId = id;
                break;
            case "RoomList":
                var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));

                MainThreadDispatcher.Post(_ => _roomsSubject.OnNext(rooms.RoomList), null);
                break;
            case "SetColor":
                GameManager.Instance.MyColor = (GameManager.TrunpColor)Enum.Parse(typeof(GameManager.TrunpColor), json["SetColor"].ToString());

                break;  
            case "GameSceneLoad":
                MainThreadDispatcher.Post(_ => GameManager.Instance.GameSceneStart.OnNext(Unit.Default), null);
                break;
            case "GameStart":
                GameManager.Instance.GameStart.OnNext(Unit.Default);
                break;
            default:
                Debug.LogError("送られてきたデータがおかしいです");
                break;
        }

    }

    public void SendJsonMessege(Messege messege)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(messege));

        Client.Send(buffer, buffer.Length);
    }

    public struct Messege
    {
        public string Name;

        public SendMesageState State;

        public int PlayerId;

        public string Method;

        public Messege(string name, SendMesageState mesageState, int playerId = 0, string method = null)
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
        var rogout = new Messege("RogOut", SendMesageState.RogOut, PlayerId);

        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(rogout));

        Client.Send(buffer, buffer.Length);
        Client.Close();
        thread.Abort();
    }
}
