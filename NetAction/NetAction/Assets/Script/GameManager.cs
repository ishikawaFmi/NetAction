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
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject _myDeckPanel;

    [SerializeField] GameObject _cardPrefab;

    [SerializeField] GameObject _fieldPanel;

    [SerializeField] Sprite[] _spriteList;

    public Subject<Unit> GameStart = new Subject<Unit>();

    public Subject<Unit> NewCardDraw = new Subject<Unit>();

    public Subject<Unit> Cheak = new Subject<Unit>();

    List<Trump> _trumpList = new List<Trump>();

    public Card CurrentCard;

    public List<Card> MyDeakCardList = new List<Card>();

    public List<Card> FieldCardList = new List<Card>();

    bool _isNotCard = false;

    public int EnemyTrumpCount = 26;
    public enum TrunpColor
    {
        Black,
        Red,
        None,
    }

    public static TrunpColor MyColor = TrunpColor.None;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }
    async void Start()
    {
        _spriteList = Resources.LoadAll<Sprite>("Image/PlayingCards");

        NewCardDraw.Subscribe(_ => IncetanceCard(Draw()));

        Cheak.Subscribe(_ => CardChaeck());
        Cheak.Subscribe(_ => WinChack());

        GameStart.Subscribe(_ => Debug.Log("GameStart"));
        GameStart.Subscribe(_ => DeckSetUp());
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NewCardDraw.OnNext(Unit.Default));
        GameStart.Subscribe(_ => NetWorkIncetanceCard(Draw()));

        await UniTask.WaitUntil(() => FieldCardList.Count == 2);
        Cheak.OnNext(Unit.Default);
    }

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
            else
            {
                Debug.LogError("カラーが設定されていないです");
            }
        }
        _trumpList = trumps.OrderBy(x => Guid.NewGuid()).ToList();
    }

    Trump Draw()
    {
        var trump = _trumpList[0];
        _trumpList.Remove(trump);
        return trump;
    }

    public Sprite ReturnCardSprite(Trump.Suit suit, int index) => _spriteList.ToList().Find(x => x.name == $"{suit}{index}");//スートとインデックスに応じたカードのスプライトを返す

    /// <summary>
    /// デッキにカードを生成する
    /// </summary>
    /// <param name="trump"></param>
    void IncetanceCard(Trump trump)
    {
        if (trump != null)
        {
            var card = Instantiate(_cardPrefab, _myDeckPanel.transform).GetComponent<Card>();
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
        var card = Instantiate(_cardPrefab, _fieldPanel.transform).GetComponent<Card>();
        var sprite = ReturnCardSprite(suit, index);
        card.SetCard(sprite, suit, index, false);

        if (MyColor == TrunpColor.Black && suit == Trump.Suit.Diamond || suit == Trump.Suit.Heart)
        {
            EnemyTrumpCount--;
        }
        else if (MyColor == TrunpColor.Red && suit == Trump.Suit.Spade || suit == Trump.Suit.Club)
        {
            EnemyTrumpCount--;
        }

        FieldCardList.Add(card);
    }

    /// <summary>
    /// サーバーにカードを生成するJsonを送る
    /// </summary>
    /// <param name="trump"></param>
    async void NetWorkIncetanceCard(Trump trump)
    {
        if (trump != null)
        {
            await NetWorkManager.Incetance.SendJsonMessege(new NetWorkManager.Messege("IncetanceCard", NetWorkManager.SendMesageState.MetHod, NetWorkManager.Incetance.PlayerId, JsonUtility.ToJson(trump)));

        }

    }


    /// <summary>
    /// 出したカード合ってたらカードを変更しドローする
    /// </summary>
    /// <param name="beforeSuit"></param>
    /// <param name="beforeIndex"></param>
    /// <param name="afterSuit"></param>
    /// <param name="afterIndex"></param>
    public void ChengeCard(Trump.Suit beforeSuit, int beforeIndex, Trump.Suit afterSuit, int afterIndex)
    {
        if (CurrentCard != null)
        {
            if (CurrentCard.CurrentSuit == afterSuit && CurrentCard.Index == afterIndex)
            {
                Destroy(CurrentCard.gameObject);
                MyDeakCardList.Remove(CurrentCard);
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
                var sprite = ReturnCardSprite(afterSuit, afterIndex);
                card.SetCard(sprite, afterSuit, afterIndex, false);
            }
        }
        Cheak.OnNext(Unit.Default);
        CurrentCard = null;
    }

    public async void FieldRefresh()
    {
        foreach (var card in FieldCardList)
        {
            Destroy(card.gameObject);
        }

        FieldCardList.Clear();

        if (_trumpList.Count > 0)
        {
            NetWorkIncetanceCard(Draw());
        }
        await UniTask.WaitUntil(() => FieldCardList.Count == 2);
        Debug.Log(FieldCardList.Count + "aaa");
        Debug.Log("FieldRefresh");
        Cheak.OnNext(Unit.Default);
    }

    public void HighlightCard(Card highlightCard)
    {
        foreach (var card in MyDeakCardList)
        {
            if (card.CurrentSuit == highlightCard.CurrentSuit && card.Index == highlightCard.Index)
            {
                try
                {
                    card.CardImage.color = new Color(1, 1, 1, 1);
                }
                catch (MissingReferenceException)
                {
                    card.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }

            }
            else
            {
                try
                {
                    card.CardImage.color = new Color(1, 1, 1, 0.5f);
                }
                catch (MissingReferenceException)
                {
                    card.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                }

            }
        }
    }
    public async void CardChaeck()
    {
        bool isChaeck = false;

        for (int i = 0; i < FieldCardList.Count; i++)
        {
            for (int x = 0; x < MyDeakCardList.Count; x++)
            {
                if (FieldCardList[i].Index + 1 == MyDeakCardList[x].Index || FieldCardList[i].Index - 1 == MyDeakCardList[x].Index)
                {
                    isChaeck = true;
                }
            }
        }

        if (!isChaeck)
        {
            await NetWorkManager.Incetance.SendJsonMessege(new NetWorkManager.Messege("NotCard", NetWorkManager.SendMesageState.MetHod, NetWorkManager.Incetance.PlayerId));
        }

    }

    public async void WinChack()
    {
        if (MyDeakCardList.Count == 0)
        {
            await NetWorkManager.Incetance.SendJsonMessege(new NetWorkManager.Messege("WinChack", NetWorkManager.SendMesageState.MetHod, NetWorkManager.Incetance.PlayerId));
        }

    }

}