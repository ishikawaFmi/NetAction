using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Incetance { get; private set; }

    [SerializeField] GameObject _myDeckPanel;

    public GameObject CardPrefab;

    [SerializeField] GameObject _fieldPanel;

    [SerializeField] Sprite[] _spriteList;

    public Subject<Unit> GameStart = new Subject<Unit>();

    public Subject<Unit> NewCardDraw = new Subject<Unit>();

    public Subject<Unit> Cheak = new Subject<Unit>();

    List<Trump> _trumpList = new List<Trump>();

    public Card CurrentCard;

    public List<Card> MyDeakCardList = new List<Card>();

    public List<Card> FieldCardList = new List<Card>();

    public int EnemyTrumpCount = 26;

    public bool WinCheak = false;
    public enum TrunpColor
    {
        Black,
        Red,
        None,
    }

    public static TrunpColor MyColor = TrunpColor.None;

    void Awake()
    {
        if (Incetance == null)
        {
            Incetance = this;
        }

    }
    async void Start()
    {
        _spriteList = Resources.LoadAll<Sprite>("Image/PlayingCards");

        NewCardDraw.Subscribe(_ => IncetanceCard(Draw()));

        Cheak.Subscribe(_ => CardChaeck());
        Cheak.Subscribe(_ => GameSceneUi.Incetance.EnemyCardCreate());
        Cheak.Subscribe(_ => WinChack());

        GameStart.Subscribe(_ => Debug.Log("GameStart"));
        GameStart.Subscribe(_ => DeckSetUp());
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NetWorkIncetanceCard(Draw()));
        GameStart.Subscribe(_ => GameSceneUi.Incetance.HighlightCard());

        await UniTask.WaitUntil(() => FieldCardList.Count == 2);
        Cheak.OnNext(Unit.Default);
    }

    /// <summary>
    /// デッキを生成しシャッフルする
    /// </summary>
    void DeckSetUp()
    {
        List<Trump> trumps = new List<Trump>();
        for (int x = 1; x <= 13; x++)
        {
            if (MyColor == TrunpColor.Black)
            {
                trumps.Add(new Spade(x));
                trumps.Add(new Club(x));
            }
            else if (MyColor == TrunpColor.Red)
            {
                trumps.Add(new Diamond(x));
                trumps.Add(new Heart(x));
            }
        }
        _trumpList = trumps.OrderBy(x => Guid.NewGuid()).ToList();
    }

    /// <summary>
    /// カードをドローする
    /// </summary>
    Trump Draw()
    {
        var trump = _trumpList[0];
        _trumpList.Remove(trump);
        return trump;
    }

    /// <summary>
    /// 変更の条件を満たしているか確認する
    /// </summary>
    /// <param name="befor"></param>
    /// <param name="after"></param>
    /// <returns></returns>
    public bool ChangeCheak(int befor, int after)
    {
        if (befor + 1 == after || befor - 1 == after || befor == 13 && after == 1 || befor == 1 && after == 13)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Sprite ReturnCardSprite(Trump.Suit suit, int index) => _spriteList.ToList().Find(x => x.name == $"{suit}{index}");

    /// <summary>
    /// カードを生成する
    /// </summary>
    /// <param name="trump"></param>
    void IncetanceCard(Trump trump)
    {
        if (trump != null)
        {
            var card = Instantiate(CardPrefab, _myDeckPanel.transform).GetComponent<Card>();
            var sprite = ReturnCardSprite(trump.MySuit, trump.CardIndex);
            card.SetCard(sprite, trump.MySuit, trump.CardIndex);

            MyDeakCardList.Add(card);
        }

    }

    /// <summary>
    /// フィールドにカードを生成する
    /// </summary>
    /// <param name="suit"></param>
    /// <param name="index"></param>
    public void FieldIncetanceCard(Trump.Suit suit, int index)
    {
        if (FieldCardList.Count <= 2)
        {
            var card = Instantiate(CardPrefab, _fieldPanel.transform).GetComponent<Card>();
            var sprite = ReturnCardSprite(suit, index);
            card.SetCard(sprite, suit, index, false);

            if (MyColor == TrunpColor.Black)
            {
                if (suit == Trump.Suit.Diamond || suit == Trump.Suit.Heart)
                {
                    EnemyTrumpCount--;
                }

            }
            else if (MyColor == TrunpColor.Red)
            {
                if (suit == Trump.Suit.Spade || suit == Trump.Suit.Club)
                {
                    EnemyTrumpCount--;
                    Debug.Log(EnemyTrumpCount);
                }

            }

            FieldCardList.Add(card);
        }

    }

    /// <summary>
    /// サーバーのカードを生成する関数に情報を渡す
    /// </summary>
    /// <param name="trump"></param>
    void NetWorkIncetanceCard(Trump trump)
    {
        if (trump != null)
        {
            var card = new Dictionary<string, object>()
            {
                {"Suit",(int)trump.MySuit},
                {"Index",trump.CardIndex},
            };
            var cardJson = MiniJSON.Json.Serialize(card);
            WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege("IncetanceCard", WebSocketManager.Messege.MessegeState.Game, cardJson));

        }

    }


    /// <summary>
    /// フィールドのカードを手札のカードと交換する
    /// </summary>
    /// <param name="beforeSuit"></param>
    /// <param name="beforeIndex"></param>
    /// <param name="afterSuit"></param>
    /// <param name="afterIndex"></param>
    public void ChengeCard(Trump.Suit beforeSuit, int beforeIndex, Trump.Suit afterSuit, int afterIndex)
    {
        if (CurrentCard != null)//�������I������J�[�h��������
        {
            if (CurrentCard.CurrentSuit == afterSuit && CurrentCard.Index == afterIndex)
            {
                Destroy(CurrentCard.gameObject);
                MyDeakCardList.Remove(CurrentCard);
                CurrentCard = null;
                GameSceneUi.Incetance.HighlightCard();
                if (_trumpList.Count > 0)
                {
                    NewCardDraw.OnNext(Unit.Default);
                }

            }
        }



        foreach (var card in FieldCardList)
        {
            if (card.CurrentSuit == beforeSuit && card.Index == beforeIndex)
            {
                if (ChangeCheak(card.Index, afterIndex))
                {
                    var sprite = ReturnCardSprite(afterSuit, afterIndex);
                    card.SetCard(sprite, afterSuit, afterIndex, false);

                    if (MyColor == TrunpColor.Black)
                    {
                        if (afterSuit == Trump.Suit.Diamond || afterSuit == Trump.Suit.Heart)
                        {
                            EnemyTrumpCount--;
                        }

                    }
                    else if (MyColor == TrunpColor.Red)
                    {
                        if (afterSuit == Trump.Suit.Spade || afterSuit == Trump.Suit.Club)
                        {
                            EnemyTrumpCount--;
                            Debug.Log(EnemyTrumpCount);
                        }

                    }
                }
            }
        }
        Cheak.OnNext(Unit.Default);
    }

    /// <summary>
    /// お互いにカードを出せない場合はフィールドのカードを破棄し
    /// 互いに一枚デッキから出す
    /// </summary>
    public async void FieldRefresh()
    {
        if (MyDeakCardList.Count == 0)
        {
            WinChack();
            return;
        }

        foreach (var card in FieldCardList)
        {
            Destroy(card.gameObject);
        }

        FieldCardList.Clear();

        if (_trumpList.Count <= 0)
        {
            var myCard = MyDeakCardList[0];

            var trump = new Dictionary<string, object>()
            {
                {"Suit",(int)myCard.CurrentSuit},
                {"Index",myCard.Index},
            };

            var cardJson = MiniJSON.Json.Serialize(trump);
            WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege("IncetanceCard", WebSocketManager.Messege.MessegeState.Game, cardJson));

            Destroy(myCard.gameObject);
            MyDeakCardList.Remove(myCard);


        }
        else if (_trumpList.Count <= 0 && MyDeakCardList.Count == 0)
        {
            WinChack();
            return;
        }
        else
        {
            var card = Draw();
            if (card != null)
            {
                NetWorkIncetanceCard(card);
            }
        }

        await UniTask.WaitUntil(() => FieldCardList.Count == 2);
        Cheak.OnNext(Unit.Default);
    }



    /// <summary>
    /// カードを出せるかチェックする
    /// </summary>
    public void CardChaeck()
    {
        bool isChaeck = false;

        for (int i = 0; i < FieldCardList.Count; i++)
        {
            for (int x = 0; x < MyDeakCardList.Count; x++)
            {
                if (ChangeCheak(FieldCardList[i].Index, MyDeakCardList[x].Index))
                {
                    isChaeck = true;
                }
            }
        }

        if (!isChaeck)
        {
            WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege("NotCard", WebSocketManager.Messege.MessegeState.Game));
        }
    }

    /// <summary>
    /// 勝敗の確認
    /// </summary>
    public void WinChack()
    {
        if (MyDeakCardList.Count == 0 && !WinCheak)
        {
            WebSocketManager.Incetance.WebSocketSendMessege(new WebSocketManager.Messege("WinChack", WebSocketManager.Messege.MessegeState.Game));
            WinCheak = true;
        }
    }
}