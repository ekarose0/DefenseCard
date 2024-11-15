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
    //해당 변수가 증가함에 따라 문양을 활성화
    [SerializeField] int curRound=0;
    //현재 뽑은 카드의 순서
    [SerializeField] int drawIndex = 0;
    [Space]

    [Header("Deck Info")]
    //모든 카드의 정보를 가진 List
    [SerializeField] List<CardSO> fullDeck = new List<CardSO>(4);
    //현재 활성화된 문양의 카드의 정보를 가진 List
    [SerializeField] List<CardSO> activeDeck = new List<CardSO>(4);
    //활성화된 문양을 랜덤으로 섞어 직접 사용되는 List
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
        [SerializeField] public string type; // Inspector에 표시
        [SerializeField] public int value;   // Inspector에 표시
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
        //문양 추가  
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

    // 모든 문양 카드를 fullDeck에 추가
    void InitializeFullDeck()
    {
        fullDeck.Add(clubsSO);
        fullDeck.Add(diamondsSO);
        fullDeck.Add(heartsSO);
        fullDeck.Add(spadesSO);
    }

    //현재 라운드 수의 따라 특정 문양 활성화 (덱과 손 패 업데이트)
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

    //활성화된 문양들의 카드들로 덱 채우기
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

    //덱 랜덤으로 섞기
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

    //카드 뽑기
    public CardInfo PopCard()
    {
        // 인덱스 순환 방식으로 카드 뽑기
        CardInfo cardInfo = cardDeck[drawIndex];
        drawIndex++;

        if (drawIndex >= cardDeck.Count)
        {
            ShuffleDeck();
            drawIndex = 0;  
        }

        return cardInfo;
    }

    //카드 인스턴스 생성 및 패에 추가
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

    //카드 위치 이동
    void PlaceCardAtPosition(GameObject cardObject, int positionIndex)
    {
            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();

            RectTransform targetPosition = cardPositions[positionIndex].GetComponent<RectTransform>();

            cardRectTransform.anchoredPosition = targetPosition.anchoredPosition;

        cardObject.GetComponent<ThisCardData>().originPos = cardRectTransform.anchoredPosition;
    }

    // 특정 위치의 카드를 교체
    public void ReplaceCard(bool isBatchReplacement = false)
    {
        bool cardReplaced = false;

        for (int i = 0; i < myHand.Count; i++)
        {
            if (myHand[i].isSelected)
            {
                // 대량 교체가 아닌 경우에만 돈 감소
                if (!isBatchReplacement)
                {
                    if (GameManager.Instance.money <= 0)
                        return;

                    GameManager.Instance.ChangeMoney(-1);
                }

                // 기존 카드 오브젝트 삭제
                Destroy(myHand[i].gameObject);

                // 새로운 카드 생성
                var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                cardObject.transform.SetParent(canvasTransform, false);

                var cardData = cardObject.GetComponent<ThisCardData>();
                cardData.Setup(PopCard());

                // 기존 위치에 카드 배치
                PlaceCardAtPosition(cardObject, i);
                myHand[i] = cardData;

                cardReplaced = true; // 카드가 교체되었음을 표시
            }
        }
        SaveHandToJson();
        if (!cardReplaced)
        {
            Debug.Log("교체할 카드가 선택되지 않았습니다.");
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
    AssetDatabase.Refresh(); // 에디터 파일 갱신
#endif
    }
}
