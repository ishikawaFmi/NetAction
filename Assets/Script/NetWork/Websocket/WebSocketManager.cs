using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;


public class WebSocketManager : MonoBehaviour
{
    static public WebSocketManager Incetance { get; private set; }

    [SerializeField]
    string _ipAddres = "13.231.242.11";

    [SerializeField]
    int _port = 5001;

    [SerializeField]
    string _id;

    WebSocket _ws;

    [DllImport("__Internal")]
    private static extern void Connect(string server);

    [DllImport("__Internal")]
    private static extern string GetMessage();

    [DllImport("__Internal")]
    private static extern void WSSendMessage(string messege);

    public Subject<Unit> GameSceneLoad = new Subject<Unit>();

    private void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        GameSceneLoad.Subscribe(_ => SceneManager.Incetance.GameSceneLoad());
        GameSceneLoad.Subscribe(_ => WebSocketSendMessege(new Messege("GameScene", Messege.MessegeState.Room)));

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Connect($"ws://{_ipAddres}:{_port}");//サーバーに接続する
            StartCoroutine(ReciveMessege());//サーバーからの受信を開始する
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            _ws = new WebSocket($"ws://{_ipAddres}:{_port}");//サーバーに接続する
            _ws.Connect();
            _ws.OnMessage += (sender, e) => Sort(e.Data);//サーバーからの受信を開始する

        }
    }

#if UNITY_WEBGL
    /// <summary>
    /// サーバーから受信し続ける
    /// </summary>
    IEnumerator ReciveMessege()
    {
        while (true)
        {
            string json = GetMessage();

            if (json != null)
            {
                Debug.Log(json);
                Sort(json);
            }

            yield return null;
        }
    }
#endif

    /// <summary>
    /// サーバーから送られてきたJsonをStateごとに処理する
    /// </summary>
    /// <param name="data">サーバーから送られてきたJsonのデータ</param>
    void Sort(string data)
    {
        var json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(data);//JsonをDictionaryに変換する

        switch (json["State"])
        {
            case "Rogin"://サーバーからIdを取得する
                _id = (string)json["Id"];
                break;
            case "RoomList"://サーバーから送られてきたルームのデータを表示する
                var rooms = JsonUtility.FromJson<Rooms>(MiniJSON.Json.Serialize(json));

                RoomListView.Incetance.RoomListSetup(rooms.RoomList);
                break;
            case "RoomDelete":
                RoomListView.Incetance.DeleteRoom();
                break;
            case "GameSceneLoad"://ゲームシーンへ遷移する
                GameSceneLoad.OnNext(Unit.Default);
                break;
            case "GameStart"://ゲームをスタートさせる
                GameManager.Incetance.GameStart.OnNext(Unit.Default);
                break;
            case "IncetanceCard"://フィールドにカードを生成する
                var index = int.Parse(json["Index"].ToString());
                var suit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["Suit"].ToString());

                GameManager.Incetance.FieldIncetanceCard(suit, index);
                break;
            case "ChengeCard"://フィールドのカードと手札を交換する
                var beforeIndex = int.Parse(json["BeforeIndex"].ToString());
                var beforeSuit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["BeforeSuit"].ToString());
                var afterIndex = int.Parse(json["AftereIndex"].ToString());
                var afterSuit = (Trump.Suit)Enum.Parse(typeof(Trump.Suit), json["AfterSuit"].ToString());

                GameManager.Incetance.ChengeCard(beforeSuit, beforeIndex, afterSuit, afterIndex);
                break;
            case "NotCard"://自分と相手の出せるカードがなかったためフィールドカードをリセットしてお互い1枚ドローする
                GameManager.Incetance.FieldRefresh();
                break;
            case "CheakWin":
                if (GameManager.Incetance.EnemyTrumpCount == 0)//相手の残りカードが0だったら相手の勝利
                {
                    WebSocketSendMessege(new Messege("SendWin", Messege.MessegeState.Game));
                    GameManager.Incetance.WinCheak = true;
                }
                else//同期ミスのためタイトルへ
                {
                    SceneManager.Incetance.TitleScene();
                }
                break;
            case "SendWin"://リザルトシーンへ遷移して勝敗を表示する
                SceneManager.Incetance.ResultScene();
                ResultSceneManager.ResultString = (string)json["WinOrLose"];
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// サーバーにメッセージを送る
    /// </summary>
    /// <param name="messge">送るメッセージの情報</param>
    public void WebSocketSendMessege(Messege messge)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WSSendMessage(JsonUtility.ToJson(messge));
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            _ws.Send(JsonUtility.ToJson(messge));
        }
    }

    /// <summary>
    /// サーバーに送る情報
    /// </summary>
    public struct Messege
    {
        public string MethodName;

        public MessegeState State;

        public string MethodData;

        public enum MessegeState
        {
            NetWork,
            Room,
            Game,
        }

        /// <param name="methodName">サーバーの呼ぶ関数名</param>
        /// <param name="messegeState">サーバーのメッセージ種類</param>
        /// <param name="methodData">関数のデータ</param>
        public Messege(string methodName, MessegeState messegeState, string methodData = null)
        {
            this.State = messegeState;
            this.MethodName = methodName;
            this.MethodData = methodData;
        }
    }

    /// <summary>
    /// ゲーム終了時に呼ぶ 
    /// </summary>
    public void Rogout()
    {
        WebSocketSendMessege(new Messege("Rogout", Messege.MessegeState.NetWork));
        StopCoroutine(ReciveMessege());
    }

    private void OnApplicationQuit()
    {
        Rogout();
        _ws.Close();
    }
}
