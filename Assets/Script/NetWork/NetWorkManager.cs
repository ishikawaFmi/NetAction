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
    string _ipAddres = "13.231.180.226";

    [SerializeField]
    int _port = 50000;

    public Guid PlayerId;

    public UdpClient Client;

    CancellationTokenSource _cancellationToken;

    //�Q�[���V�[���Ɉڍs���ɌĂ�
    public Subject<Unit> GameSceneStart = new Subject<Unit>();

    public enum SendMesageState
    {
        RogIn,
        RogOut,
        MetHod,
        NetWorkMetHod,
    }
    void Start()
    {
        GameSceneStart.Subscribe(_ => SceneManager.Incetance.GameSceneLoad());
        GameSceneStart.Subscribe(_ =>  Incetance.SendJsonMessege(new Messege("GameScene", SendMesageState.NetWorkMetHod, PlayerId)));
    }

    async void Conect()
    {
        var s = Scheduler.MainThread;//宣言しておかないとMainThread

        Incetance.PlayerId = Guid.NewGuid();

        Client = new UdpClient();

        IPAddress ipAddres = IPAddress.Parse(_ipAddres);

        while (!Client.Client.Connected)
        {
            Client.Connect(ipAddres, _port);
        }

      

        if (!Client.Client.Connected)
        {
            Debug.LogError("err");
        }
        else
        {
            Debug.Log("sec");
        }

        _cancellationToken = new CancellationTokenSource();

        ThreadMetHod(_cancellationToken.Token);

        await UniTask.WaitUntil(() => Client.Client.Connected);

        SendJsonMessege(new Messege("Rogin", NetWorkManager.SendMesageState.RogIn, NetWorkManager.Incetance.PlayerId));
    }

    /// <summary>
    /// �N���C�A���g����f�[�^���M����
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
    /// ��M�����f�[�^��̏��������
    /// </summary>
    /// <param name="data"></param>
     void Receive(byte[] data)
    {
        var x = Encoding.UTF8.GetString(data);

        Dictionary<string, object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(x);//�����Ă����f�[�^��Dictionary�ɕϊ�����

        switch ((string)json["State"])
        {
            case "RoomList":
                var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));

                MainThreadDispatcher.Post(_ => RoomListView.Incetance.RoomListSetup(rooms.RoomList), null);
                break;
            case "RoomDelete":
                MainThreadDispatcher.Post(_ => RoomListView.Incetance.DeleteRoom(), null);
                break;
            case "SetColor":
                GameManager.MyColor = (GameManager.TrunpColor)Enum.Parse(typeof(GameManager.TrunpColor), json["SetColor"].ToString());
                break;
            case "GameSceneLoad":
                MainThreadDispatcher.Post(_ => GameSceneStart.OnNext(Unit.Default), null);
                break;
            case "GameStart":
                MainThreadDispatcher.Post(_ => GameManager.Incetance.GameStart.OnNext(Unit.Default), null);
                break;
            case "IncetanceCard":
                var index = int.Parse(json["Index"].ToString());
                var suit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["Suit"].ToString());
                MainThreadDispatcher.Post(_ => GameManager.Incetance.FieldIncetanceCard(suit, index), null);

                break;
            case "ChengeCard":
                var beforeIndex = int.Parse(json["BeforeIndex"].ToString());
                var beforeSuit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["BeforeSuit"].ToString());
                var afterIndex = int.Parse(json["AftereIndex"].ToString());
                var afterSuit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["AfterSuit"].ToString());

                MainThreadDispatcher.Post(_ => GameManager.Incetance.ChengeCard(beforeSuit, beforeIndex, afterSuit, afterIndex), null);

                break;
            case "NotCard":
                MainThreadDispatcher.Post(_ => GameManager.Incetance.FieldRefresh(), null);
                break;      
            case "CheakWin":
                if (GameManager.Incetance.EnemyTrumpCount == 0)
                {
                   SendJsonMessege(new Messege("SendWin", NetWorkManager.SendMesageState.MetHod, NetWorkManager.Incetance.PlayerId));
                   GameManager.Incetance.WinCheak = true;
                }
                break;
            case "SendWin":
                
                MainThreadDispatcher.Post(_ => SceneManager.Incetance.ResultScene(), null);
                ResultSceneManager.ResultString = (string)json["WinOrLose"];
                break;
            default:
                Debug.LogError("�����Ă����f�[�^�����������ł�");
                break;
        }

    }

    public void SendJsonMessege(Messege messege)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(messege));
        if (Client != null)
        {
            Client.SendAsync(buffer, buffer.Length);
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
    /// �A�v���P�[�V�����I����
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("aaaa");
        var rogout = new Messege("RogOut", SendMesageState.RogOut, PlayerId);
        SendJsonMessege(rogout);

        _cancellationToken.Cancel();
    }
}
