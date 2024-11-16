using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private const string JsonFilePath = ResourceDefine.CurrontJsonData;

    [Header("CardSO")]
    [SerializeField] CardSO clubsSO;
    [SerializeField] CardSO heartsSO;
    [SerializeField] CardSO diamondsSO;
    [SerializeField] CardSO spadesSO;

    [Space]
    [SerializeField] int curRound = 0; // 문양 활성화 단계
    [SerializeField] int drawIndex = 0; // 덱에서 뽑은 카드의 순서

    [Space]
    [Header("Deck Info")]
    [SerializeField] List<CardSO> fullDeck = new List<CardSO>(4);
    [SerializeField] List<CardSO> activeDeck = new List<CardSO>(4);
    [SerializeField] List<CardInfo> cardDeck = new List<CardInfo>(52);

    [Space]
    [Header("Card Configuration")]
    [SerializeField] public Transform canvasTransform;
    [SerializeField] public Transform SingleFrontCard_MiddleTransform;
    [SerializeField] public Transform SingleFrontCard_LastTransform;
    [SerializeField] public Transform[] cardPositions;
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public List<ThisCardData> myHand;

    [Space]
    [Header("카드 교체 비용")]
    private const int CardCost = 1;

    [System.Serializable]
    public class Card
    {
        public int id;
        public string type;
        public int value;
    }

    [System.Serializable]
    public class CardCollection
    {
        public List<Card> cards;
    }

    private void Start()
    {
        InitializeFullDeck();
        GameManager.Instance.UIManager.UpdateRoundText(curRound);
    }

    /// <summary>
    /// 초기 5장의 카드를 생성
    /// </summary>
    public void InitializeStartingHand()
    {
        UpdateActiveDeck(false);

        for (int i = 0; i < 5; i++)
        {
            AddCard(i);
        }
    }

    /// <summary>
    /// 특정 위치에 새 카드를 생성
    /// </summary>
    private void AddCard(int positionIndex)
    {
        var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        // 부모를 SingleFrontCard_MiddleTransform으로 설정
        cardObject.transform.SetParent(SingleFrontCard_MiddleTransform, false);

        var cardData = cardObject.GetComponent<ThisCardData>();
        cardData.Setup(PopCard());

        // 카드 위치 설정
        PlaceCardAtPosition(cardObject, positionIndex);

        // 카드 목록에 추가
        myHand.Add(cardData);
    }

    /// <summary>
    /// 카드 위치를 설정
    /// </summary>
    public void PlaceCardAtPosition(GameObject cardObject, int positionIndex)
    {
        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        RectTransform targetPosition = cardPositions[positionIndex].GetComponent<RectTransform>();

        cardRectTransform.anchoredPosition = targetPosition.anchoredPosition;
        cardObject.GetComponent<ThisCardData>().originPos = cardRectTransform.anchoredPosition;
    }

    /// <summary>
    /// 교체된 카드 데이터를 새로 생성
    /// </summary>
    public void GenerateReplacedCards(List<int> replacedCardIndices)
    {
        foreach (int index in replacedCardIndices)
        {
            var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            cardObject.transform.SetParent(SingleFrontCard_MiddleTransform, false);

            var cardData = cardObject.GetComponent<ThisCardData>();
            cardData.Setup(PopCard());

            PlaceCardAtPosition(cardObject, index);
            myHand[index] = cardData;
        }
    }

    /// <summary>
    /// 모든 앞면 카드를 SingleFrontCard_LastTransform으로 이동
    /// </summary>
    public void MoveFrontCardsToLast()
    {
        foreach (var card in myHand)
        {
            if (card != null)
            {
                card.gameObject.transform.SetParent(SingleFrontCard_LastTransform, false);
            }
        }
    }

    /// <summary>
    /// 카드 교체
    /// </summary>
    public void ReplaceCard(bool isBatchReplacement = false)
    {
        bool cardReplaced = false;
        List<int> replacedCardIndices = new List<int>();
        List<GameObject> cardsToDelete = new List<GameObject>();

        for (int i = 0; i < myHand.Count; i++)
        {
            if (myHand[i].isSelected)
            {
                // 비용 계산
                if (!isBatchReplacement && !GameManager.Instance.CanAfford(CardCost))
                {
                    GameManager.Instance.UIManager.ShowWarning("Not enough coins to replace the card!");
                    return;
                }

                if (!isBatchReplacement)
                {
                    GameManager.Instance.ChangeMoney(-CardCost);
                }

                // 교체 대상 추가
                cardsToDelete.Add(myHand[i].gameObject);
                replacedCardIndices.Add(i);
                cardReplaced = true;
            }
        }

        if (cardReplaced)
        {
            GameManager.Instance.UIManager.UpdateNoticeText("In Progress...");
            GameManager.Instance.AnimeManager.StartReplaceAnimation(cardsToDelete, replacedCardIndices);
        }
        else
        {
            GameManager.Instance.UIManager.ShowWarning("No cards selected for replacement!");
        }
    }

    /// <summary>
    /// 모든 문양의 카드를 초기화
    /// </summary>
    private void InitializeFullDeck()
    {
        fullDeck.Add(clubsSO);
        fullDeck.Add(diamondsSO);
        fullDeck.Add(heartsSO);
        fullDeck.Add(spadesSO);
    }

    /// <summary>
    /// 문양 활성화에 따라 덱 설정
    /// </summary>
    private void UpdateActiveDeck(bool selectAll = true)
    {
        switch (curRound)
        {
            case 0: activeDeck.Add(clubsSO); break;
            case 1: activeDeck.Add(heartsSO); break;
            case 2: activeDeck.Add(diamondsSO); break;
            case 3: activeDeck.Add(spadesSO); break;
        }

        SetupDeck();

        if (selectAll)
        {
            foreach (var card in myHand)
                card.isSelected = true;

            ReplaceCard(true);
        }
    }

    /// <summary>
    /// 활성화된 문양의 카드를 덱에 추가
    /// </summary>
    private void SetupDeck()
    {
        cardDeck.Clear();

        foreach (CardSO card in activeDeck)
        {
            cardDeck.AddRange(card.cardInfo);
        }

        ShuffleDeck();
        drawIndex = 0;
    }

    /// <summary>
    /// 덱 섞기
    /// </summary>
    private void ShuffleDeck()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            int rand = Random.Range(i, cardDeck.Count);
            (cardDeck[i], cardDeck[rand]) = (cardDeck[rand], cardDeck[i]);
        }
    }

    /// <summary>
    /// 카드 뽑기
    /// </summary>
    private CardInfo PopCard()
    {
        CardInfo cardInfo = cardDeck[drawIndex++];
        if (drawIndex >= cardDeck.Count)
        {
            ShuffleDeck();
            drawIndex = 0;
        }

        return cardInfo;
    }

    public void ClearCurrentCards()
    {
        // 현재 카드 삭제 (연출 없이 바로 삭제)
        foreach (var card in myHand)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        myHand.Clear();
    }

    public void IncrementRound()
    {
        curRound++;
        GameManager.Instance.UIManager.UpdateRoundText(curRound);
    }


    /// <summary>
    /// 카드 선택 상태를 설정합니다.
    /// </summary>
    /// <param name="isEnabled">선택 상태를 설정할 값 (true: 활성화, false: 비활성화)</param>
    public void SetCardSelection(bool isEnabled)
    {
        foreach (var card in myHand)
        {
            if (card != null)
            {
                card.isSelectionEnabled = isEnabled;
            }
        }
    }

    public int CheckCurRound()
    {
        return curRound;
    }

    public void SaveHandToJson()
    {
        CardCollection cardCollection = new CardCollection { cards = new List<Card>() };

        int i = 0;

        foreach (var cardData in myHand)
        {
            Card card = new Card
            {
                id = i++,
                type = cardData.cardInfo.cardType.ToString(),
                value = cardData.cardInfo.cardNum
            };
            cardCollection.cards.Add(card);
        }

        string jsonContent = JsonUtility.ToJson(cardCollection, true);
        File.WriteAllText(JsonFilePath, jsonContent);

#if UNITY_EDITOR
        AssetDatabase.Refresh(); // 에디터 파일 갱신
#endif
    }
}
