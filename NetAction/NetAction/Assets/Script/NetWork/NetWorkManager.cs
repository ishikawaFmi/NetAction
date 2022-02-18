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
            Conect();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    [SerializeField]
    string _host = "localHost";

    [SerializeField]
    int _port = 50000;

    public Guid PlayerId;

    public UdpClient Client;

    Thread _thread;

    CancellationTokenSource _cancellationToken;

    //ゲームシーンに移行時に呼ぶ
    public Subject<Unit> GameSceneStart = new Subject<Unit>();

    //ゲームシーンをプリロードして置くための変数
    public AsyncOperation GameSceneAsync;


    public enum SendMesageState
    {
        RogIn,
        RogOut,
        MetHod,
        NetWorkMetHod,
    }
    void Start()
    {
        GameSceneStart.Subscribe(_ => GameSceneAsync.allowSceneActivation = true);
        GameSceneStart.Subscribe(async _ => await Incetance.SendJsonMessege(new Messege("GameScene", SendMesageState.NetWorkMetHod, PlayerId)));
    }

    async void Conect()
    {
        Incetance.PlayerId = Guid.NewGuid();

        Client = new UdpClient();

        while (!Client.Client.Connected)
        {
            Client.Connect(_host, _port);
        }

        var s = Scheduler.MainThread;//アクセスしておかないとMainThreadDispatcherが使えない

        if (!Client.Client.Connected)
        {
            Debug.LogError("err");
        }
        else
        {
            Debug.Log("sec");
        }

        _cancellationToken = new CancellationTokenSource();

        // _thread = new Thread(new ThreadStart(ThreadMetHod));
        ThreadMetHod(_cancellationToken.Token);

        await UniTask.WaitUntil(() => Client.Client.Connected);

        await SendJsonMessege(new Messege("Rogin", NetWorkManager.SendMesageState.RogIn, NetWorkManager.Incetance.PlayerId));
        SceneManager.Incetance.GameSceneLoadAsync();
    }

    /// <summary>
    /// クライアントからデータを受信する
    /// </summary>
    async void ThreadMetHod(CancellationToken cancellationToken)
    {
        await UniTask.SwitchToThreadPool();

        cancellationToken.Register(() => Client.Close());
        while (true)
        {
            IPEndPoint iP = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                if (Client.Client != null)
                {
                    if (Client.Client.Connected)
                    {
                        byte[] data = Client.Receive(ref iP);
                        Receive(data);
                    }
                }


            }
            catch (SocketException e)
            {
                Debug.Log(e);
            }
        }
    }
    /// <summary>
    /// 受信したデータをの処理をする
    /// </summary>
    /// <param name="data"></param>
    async void Receive(byte[] data)
    {
        var x = Encoding.UTF8.GetString(data);

        Dictionary<string, object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(x);//送られてきたデータをDictionaryに変換する

        switch ((string)json["State"])
        {
            case "RoomList":
                var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));

                MainThreadDispatcher.Post(_ => RoomListView.Instance.RoomListSetup(rooms.RoomList), null);
                break;
            case "RoomDelete":
                if (RoomListView.Instance.IsInRoom && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game")
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
                }
                else if (RoomListView.Instance.IsInRoom)
                {
                    RoomListView.Instance.LeftRoom();
                }
                break;
            case "SetColor":
                GameManager.MyColor = (GameManager.TrunpColor)Enum.Parse(typeof(GameManager.TrunpColor), json["SetColor"].ToString());
                break;
            case "GameSceneLoad":
                MainThreadDispatcher.Post(_ => GameSceneStart.OnNext(Unit.Default), null);
                break;
            case "GameStart":
                MainThreadDispatcher.Post(_ => GameManager.Instance.GameStart.OnNext(Unit.Default), null);
                break;
            case "IncetanceCard":
                var index = int.Parse(json["Index"].ToString());
                var suit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["Suit"].ToString());
                MainThreadDispatcher.Post(_ => GameManager.Instance.FieldIncetanceCard(suit, index), null);

                break;
            case "ChengeCard":
                var beforeIndex = int.Parse(json["BeforeIndex"].ToString());
                var beforeSuit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["BeforeSuit"].ToString());
                var afterIndex = int.Parse(json["AftereIndex"].ToString());
                var afterSuit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["AfterSuit"].ToString());

                MainThreadDispatcher.Post(_ => GameManager.Instance.ChengeCard(beforeSuit, beforeIndex, afterSuit, afterIndex), null);

                break;
            case "NotCard":
                MainThreadDispatcher.Post(_ => GameManager.Instance.FieldRefresh(), null);
                break;      
            case "CheakWin":
                if (GameManager.Instance.EnemyTrumpCount == 0)
                {
                  await SendJsonMessege(new Messege("SendWin", NetWorkManager.SendMesageState.MetHod, NetWorkManager.Incetance.PlayerId));
                }
                break;
            case "SendWin":
                 MainThreadDispatcher.Post(_ => SceneManager.Incetance.ResultScene(), null);
                break;
            default:
                Debug.LogError("送られてきたデータがおかしいです");
                break;
        }

    }

    public async Task SendJsonMessege(Messege messege)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(messege));
        if (Client != null)
        {
            await Client.SendAsync(buffer, buffer.Length);
        }
    }

    public struct Messege
    {
        public string Name;

        public SendMesageState State;

        public string PlayerId;

        public string Method;

        public Messege(string name, SendMesageState mesageState, Guid playerId, string method = null)
        {
            Name = name;
            State = mesageState;
            var jsonPlayerId = new Dictionary<string, Guid>()
            {
               {"PlayerID" ,playerId},
            };
            PlayerId = MiniJSON.Json.Serialize(jsonPlayerId);
            Method = method;
        }
    }

    /// <summary>
    /// アプリケーション終了時
    /// </summary>
    async void OnApplicationQuit()
    {
        Debug.Log("aaaa");
        var rogout = new Messege("RogOut", SendMesageState.RogOut, PlayerId);
        await SendJsonMessege(rogout);

        _cancellationToken.Cancel();
    }
}
