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
    [SerializeField] int curRound = 0; // ���� Ȱ��ȭ �ܰ�
    [SerializeField] int drawIndex = 0; // ������ ���� ī���� ����

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
    [Header("ī�� ��ü ���")]
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
    /// �ʱ� 5���� ī�带 ����
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
    /// Ư�� ��ġ�� �� ī�带 ����
    /// </summary>
    private void AddCard(int positionIndex)
    {
        var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        // �θ� SingleFrontCard_MiddleTransform���� ����
        cardObject.transform.SetParent(SingleFrontCard_MiddleTransform, false);

        var cardData = cardObject.GetComponent<ThisCardData>();
        cardData.Setup(PopCard());

        // ī�� ��ġ ����
        PlaceCardAtPosition(cardObject, positionIndex);

        // ī�� ��Ͽ� �߰�
        myHand.Add(cardData);
    }

    /// <summary>
    /// ī�� ��ġ�� ����
    /// </summary>
    public void PlaceCardAtPosition(GameObject cardObject, int positionIndex)
    {
        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        RectTransform targetPosition = cardPositions[positionIndex].GetComponent<RectTransform>();

        cardRectTransform.anchoredPosition = targetPosition.anchoredPosition;
        cardObject.GetComponent<ThisCardData>().originPos = cardRectTransform.anchoredPosition;
    }

    /// <summary>
    /// ��ü�� ī�� �����͸� ���� ����
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
    /// ��� �ո� ī�带 SingleFrontCard_LastTransform���� �̵�
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
    /// ī�� ��ü
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
                // ��� ���
                if (!isBatchReplacement && !GameManager.Instance.CanAfford(CardCost))
                {
                    GameManager.Instance.UIManager.ShowWarning("Not enough coins to replace the card!");
                    return;
                }

                if (!isBatchReplacement)
                {
                    GameManager.Instance.ChangeMoney(-CardCost);
                }

                // ��ü ��� �߰�
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
    /// ��� ������ ī�带 �ʱ�ȭ
    /// </summary>
    private void InitializeFullDeck()
    {
        fullDeck.Add(clubsSO);
        fullDeck.Add(diamondsSO);
        fullDeck.Add(heartsSO);
        fullDeck.Add(spadesSO);
    }

    /// <summary>
    /// ���� Ȱ��ȭ�� ���� �� ����
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
    /// Ȱ��ȭ�� ������ ī�带 ���� �߰�
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
    /// �� ����
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
    /// ī�� �̱�
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
        // ���� ī�� ���� (���� ���� �ٷ� ����)
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
    /// ī�� ���� ���¸� �����մϴ�.
    /// </summary>
    /// <param name="isEnabled">���� ���¸� ������ �� (true: Ȱ��ȭ, false: ��Ȱ��ȭ)</param>
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
        AssetDatabase.Refresh(); // ������ ���� ����
#endif
    }
}
