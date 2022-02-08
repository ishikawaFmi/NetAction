using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject _myDeckPanel;

    //自分が赤だったら
    public GameObject HeartCardPrefab;
    public GameObject DiamondCardPrefab;

    //自分が黒だったら
    public GameObject SpadeCardPrefab;
    public GameObject ClubCardPrefab;

    //ゲームシーンの移行の準備する
    public Subject<Unit> GamePreparation = new Subject<Unit>();

    //ゲームシーンに移行時に呼ぶ
    public Subject<Unit> GameSceneStart = new Subject<Unit>();

    public Subject<Unit> GameStart = new Subject<Unit>();

    //ゲームシーンをプリロードして置くための変数
    public AsyncOperation GameSceneAsync;

    List<Trump> trumpList = new List<Trump>();

    public Subject<Trump> NewCardDraw = new Subject<Trump>();

    public enum TrunpColor
    {
        Black,
        Red,
        None,
    }

    public TrunpColor MyColor = TrunpColor.None;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            GamePreparation.Subscribe(_ => NetWorkManager.Incetance.SendJsonMessege(new NetWorkManager.Messege("Rogin", NetWorkManager.SendMesageState.RogIn)));
            GamePreparation.Subscribe(_ => SceneManager.Instance.GameSceneLoadAsync());

            GameSceneStart.Subscribe(_ => GameSceneAsync.allowSceneActivation = true);

            GameStart.Subscribe(_ => DeckSetUp());
            GameStart.Subscribe(_ => Shuffle());
            GameStart.Subscribe(_ => NewCardDraw.Take(4));

            NewCardDraw.Subscribe(_=> IncetanceCard(Draw()));
        }

    }

    void DeckSetUp()
    {

        for (int x = 1; x <= 13; x++)
        {
            if (MyColor == TrunpColor.Black)
            {
                trumpList.Add(new Spade(x));
                trumpList.Add(new Club(x));

            }
            else if (MyColor == TrunpColor.Red)
            {
                trumpList.Add(new Heart(x));
                trumpList.Add(new Diamond(x));
            }
            else
            {
                Debug.LogError("カラーが設定されていないです");
            }
        }
    }
    void Shuffle() => trumpList.OrderBy(x => Guid.NewGuid()).ToList();

    Trump Draw()
    {
       var trump = trumpList[0];
       trumpList.Remove(trump);
       return trump;
    }
    /// <summary>
    /// トランプに応じたカードを生成する
    /// </summary>
    /// <param name="trump"></param>
    void IncetanceCard(Trump trump)
    {
        switch (trump.MySuit)
        {
            case Trump.Suit.Heart:
                Instantiate(HeartCardPrefab, _myDeckPanel.transform).GetComponent<Heart>().CardIndex = trump.CardIndex;
                break;
            case Trump.Suit.Diamond:
             Instantiate(DiamondCardPrefab, _myDeckPanel.transform).GetComponent<Diamond>().CardIndex = trump.CardIndex;
                break;
            case Trump.Suit.Spade:
                Instantiate(SpadeCardPrefab, _myDeckPanel.transform).GetComponent<Spade>().CardIndex = trump.CardIndex;
                break;
            case Trump.Suit.Club:
                Instantiate(ClubCardPrefab, _myDeckPanel.transform).GetComponent<Club>().CardIndex = trump.CardIndex;
                break;
            case Trump.Suit.None:
                break;
            default:
                break;
        }
    }
}