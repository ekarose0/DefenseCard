using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private const string JsonFilePath = ResourceDefine.CurrontJsonData;

    [Header("CardSO")]
    [SerializeField] CardSO clubsSO;
    [SerializeField] CardSO heartsSO;
    [SerializeField] CardSO diamondsSO;
    [SerializeField] CardSO spadesSO;

    [Space]
    //�ش� ������ �����Կ� ���� ������ Ȱ��ȭ
    [SerializeField] int curRound=0;
    //���� ���� ī���� ����
    [SerializeField] int drawIndex = 0;
    [Space]

    [Header("Deck Info")]
    //��� ī���� ������ ���� List
    [SerializeField] List<CardSO> fullDeck = new List<CardSO>(4);
    //���� Ȱ��ȭ�� ������ ī���� ������ ���� List
    [SerializeField] List<CardSO> activeDeck = new List<CardSO>(4);
    //Ȱ��ȭ�� ������ �������� ���� ���� ���Ǵ� List
    [SerializeField] List<CardInfo> cardDeck = new List<CardInfo>(52);

    [Space]
    [SerializeField] Transform canvasTransform;
    [SerializeField] Transform[] cardPositions;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<ThisCardData> myHand;

    [System.Serializable]
    public class Card
    {
        public int id;
        [SerializeField] public string type; // Inspector�� ǥ��
        [SerializeField] public int value;   // Inspector�� ǥ��
    }

    [System.Serializable]
    public class CardCollection
    {
        public List<Card> cards;
    }

    void Start()
    {
        InitializeFullDeck();
        InitializeStartingHand();
        SaveHandToJson();
    }

    private void InitializeStartingHand()
    {
        UpdateActiveDeck(false);

        for (int i = 0; i < 5; i++)
            AddCard();
    }

    private void Update()
    {
        //���� �߰�  
        if (Input.GetKeyDown(KeyCode.Q))
        {
            curRound++;
            if(curRound==4)
            {
                curRound = 0;
                activeDeck.Clear();
            }
            UpdateActiveDeck();
        }
    }

    // ��� ���� ī�带 fullDeck�� �߰�
    void InitializeFullDeck()
    {
        fullDeck.Add(clubsSO);
        fullDeck.Add(diamondsSO);
        fullDeck.Add(heartsSO);
        fullDeck.Add(spadesSO);
    }

    //���� ���� ���� ���� Ư�� ���� Ȱ��ȭ (���� �� �� ������Ʈ)
    void UpdateActiveDeck(bool selectAll = true)
    {
        switch(curRound)
        {
            case 0:
                activeDeck.Add(clubsSO);
                break;
            case 1:
                activeDeck.Add(heartsSO);
                break;
            case 2:
                activeDeck.Add(diamondsSO);
                break;
            case 3:
                activeDeck.Add(spadesSO);
                break;
        }
        SetupDeck();
        if (selectAll)
        {
            foreach (ThisCardData myHand in myHand)
                myHand.isSelected = true;
            ReplaceCard(true);
        }
    }

    //Ȱ��ȭ�� ������� ī���� �� ä���
    void SetupDeck()
    {
        cardDeck.Clear();
        foreach (CardSO card in activeDeck)
        {
            for (int i = 0; i < card.cardInfo.Length; i++)
            {
                cardDeck.Add(card.cardInfo[i]);
            }
        }

        ShuffleDeck();
        drawIndex = 0;
    }

    //�� �������� ����
    private void ShuffleDeck()
    {
        for (int i = 0; i < cardDeck.Count; i++)
        {
            int rand = Random.Range(i, cardDeck.Count);
            CardInfo temp = cardDeck[i];
            cardDeck[i] = cardDeck[rand];
            cardDeck[rand] = temp;
        }
    }

    //ī�� �̱�
    public CardInfo PopCard()
    {
        // �ε��� ��ȯ ������� ī�� �̱�
        CardInfo cardInfo = cardDeck[drawIndex];
        drawIndex++;

        if (drawIndex >= cardDeck.Count)
        {
            ShuffleDeck();
            drawIndex = 0;  
        }

        return cardInfo;
    }

    //ī�� �ν��Ͻ� ���� �� �п� �߰�
    void AddCard()
    {
        var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

        cardObject.transform.SetParent(canvasTransform, false);

        cardObject.transform.SetParent(canvasTransform, false);

        var cardData = cardObject.GetComponent<ThisCardData>();
            cardData.Setup(PopCard());

        PlaceCardAtPosition(cardObject, myHand.Count);

        myHand.Add(cardData);
    }

    //ī�� ��ġ �̵�
    void PlaceCardAtPosition(GameObject cardObject, int positionIndex)
    {
            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();

            RectTransform targetPosition = cardPositions[positionIndex].GetComponent<RectTransform>();

            cardRectTransform.anchoredPosition = targetPosition.anchoredPosition;

        cardObject.GetComponent<ThisCardData>().originPos = cardRectTransform.anchoredPosition;
    }

    // Ư�� ��ġ�� ī�带 ��ü
    public void ReplaceCard(bool isBatchReplacement = false)
    {
        bool cardReplaced = false;

        for (int i = 0; i < myHand.Count; i++)
        {
            if (myHand[i].isSelected)
            {
                // �뷮 ��ü�� �ƴ� ��쿡�� �� ����
                if (!isBatchReplacement)
                {
                    if (GameManager.Instance.money <= 0)
                        return;

                    GameManager.Instance.ChangeMoney(-1);
                }

                // ���� ī�� ������Ʈ ����
                Destroy(myHand[i].gameObject);

                // ���ο� ī�� ����
                var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                cardObject.transform.SetParent(canvasTransform, false);

                var cardData = cardObject.GetComponent<ThisCardData>();
                cardData.Setup(PopCard());

                // ���� ��ġ�� ī�� ��ġ
                PlaceCardAtPosition(cardObject, i);
                myHand[i] = cardData;

                cardReplaced = true; // ī�尡 ��ü�Ǿ����� ǥ��
            }
        }
        SaveHandToJson();
        if (!cardReplaced)
        {
            Debug.Log("��ü�� ī�尡 ���õ��� �ʾҽ��ϴ�.");
        }
    }

    void SaveHandToJson()
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
