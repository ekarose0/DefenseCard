using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class CompletionDetector : MonoBehaviour
{
    private const string JsonFilePath = ResourceDefine.CurrontJsonData; // JSON 파일 경로
    private List<Card> cardDeck; // JSON에서 불러온 카드 리스트
    [SerializeField] private string bestHandDescription; // Inspector에 가장 높은 족보 명칭을 표시
    private System.DateTime lastModifiedTime; // JSON 파일의 마지막 수정 시간 기록

    // 카드 데이터를 나타내는 클래스
    [System.Serializable]
    public class Card
    {
        public int id; // 카드의 고유 ID
        public string type; // 카드의 타입 (spades, hearts, clubs, diamonds)
        public int value; // 카드의 값 (1~13)
    }

    // JSON 파일의 구조에 맞춘 카드 컬렉션 클래스
    [System.Serializable]
    public class CardCollection
    {
        public List<Card> cards;
    }

    private void Start()
    {
        // JSON 데이터를 처음 로드하고 가장 높은 족보를 Inspector에 표시
        LoadJsonData();
        bestHandDescription = EvaluateBestPokerHand(cardDeck);
    }

    private void Update()
    {
        // 파일 변경 감지 - 매 프레임마다 파일의 마지막 수정 시간을 확인
        if (File.Exists(JsonFilePath))
        {
            var currentModifiedTime = File.GetLastWriteTime(JsonFilePath);
            if (currentModifiedTime != lastModifiedTime)
            {
                lastModifiedTime = currentModifiedTime;
                LoadJsonData(); // 변경된 경우 JSON 데이터를 다시 로드
                bestHandDescription = EvaluateBestPokerHand(cardDeck); // 족보 재평가
                Debug.Log("JSON 파일이 변경되어 다시 로드되었습니다.");
            }
        }
    }

    // JSON 파일을 로드하여 카드 데이터를 초기화하는 메서드
    private void LoadJsonData()
    {
        if (File.Exists(JsonFilePath))
        {
            string jsonContent = File.ReadAllText(JsonFilePath);
            CardCollection cardCollection = JsonUtility.FromJson<CardCollection>(jsonContent);
            cardDeck = cardCollection.cards;
            lastModifiedTime = File.GetLastWriteTime(JsonFilePath); // 마지막 수정 시간 업데이트
            Debug.Log("JSON 파일 로드 성공");
        }
        else
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다.");
        }
    }

    // 가장 높은 포커 족보를 평가하는 메서드
    private string EvaluateBestPokerHand(List<Card> cards)
    {
        // 각 족보를 평가하여 가장 높은 족보의 이름을 반환
        if (FindStraightFlush(cards) != null) return "Straight Flush";
        if (FindFourOfAKind(cards) != null) return "Four of a Kind";
        if (FindFullHouse(cards) != null) return "Full House";
        if (FindFlush(cards) != null) return "Flush";
        if (FindStraight(cards) != null) return "Straight";
        if (FindThreeOfAKind(cards) != null) return "Three of a Kind";
        if (FindTwoPair(cards) != null) return "Two Pair";
        if (FindOnePair(cards) != null) return "One Pair";
        return "High Card";
    }

    // 각 족보를 평가하는 메서드들

    // Straight Flush: 같은 타입의 연속된 5장 탐색
    private List<Card> FindStraightFlush(List<Card> cards)
    {
        var flushCards = cards.GroupBy(c => c.type)
                              .Where(g => g.Count() >= 5)
                              .SelectMany(g => g)
                              .ToList();
        if (flushCards.Count < 5) return null;
        return FindStraight(flushCards);
    }

    // Four of a Kind: 같은 값의 카드 4장 탐색
    private List<Card> FindFourOfAKind(List<Card> cards)
    {
        var fourOfAKind = cards.GroupBy(c => c.value)
                               .Where(g => g.Count() == 4)
                               .SelectMany(g => g)
                               .ToList();
        return fourOfAKind.Count == 4 ? fourOfAKind : null;
    }

    // Full House: 같은 값의 카드 3장 + 2장 탐색
    private List<Card> FindFullHouse(List<Card> cards)
    {
        var threeOfAKind = cards.GroupBy(c => c.value)
                                .Where(g => g.Count() == 3)
                                .SelectMany(g => g)
                                .ToList();
        if (threeOfAKind.Count == 3)
        {
            var pair = cards.Except(threeOfAKind)
                            .GroupBy(c => c.value)
                            .Where(g => g.Count() == 2)
                            .SelectMany(g => g)
                            .ToList();
            if (pair.Count == 2) return threeOfAKind.Concat(pair).ToList();
        }
        return null;
    }

    // Flush: 같은 타입의 카드 5장 탐색
    private List<Card> FindFlush(List<Card> cards)
    {
        var flush = cards.GroupBy(c => c.type)
                         .Where(g => g.Count() >= 5)
                         .SelectMany(g => g.Take(5))
                         .ToList();
        return flush.Count == 5 ? flush : null;
    }

    // Straight: 연속된 값의 카드 5장 탐색
    private List<Card> FindStraight(List<Card> cards)
    {
        var orderedCards = cards.OrderBy(c => c.value).Distinct().ToList();
        List<Card> straight = new List<Card>();
        for (int i = 0; i < orderedCards.Count - 4; i++)
        {
            straight = orderedCards.Skip(i).Take(5).ToList();
            if (IsSequential(straight)) return straight;
        }
        return null;
    }

    // 카드가 연속된 값을 가지는지 확인
    private bool IsSequential(List<Card> cards)
    {
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i + 1].value != cards[i].value + 1) return false;
        }
        return true;
    }

    // Three of a Kind: 같은 값의 카드 3장 탐색
    private List<Card> FindThreeOfAKind(List<Card> cards)
    {
        var threeOfAKind = cards.GroupBy(c => c.value)
                                .Where(g => g.Count() == 3)
                                .SelectMany(g => g)
                                .ToList();
        return threeOfAKind.Count == 3 ? threeOfAKind : null;
    }

    // Two Pair: 2쌍의 페어 탐색
    private List<Card> FindTwoPair(List<Card> cards)
    {
        var pairs = cards.GroupBy(c => c.value)
                         .Where(g => g.Count() == 2)
                         .Take(2)
                         .SelectMany(g => g)
                         .ToList();
        return pairs.Count == 4 ? pairs : null;
    }

    // One Pair: 같은 값의 카드 2장 탐색
    private List<Card> FindOnePair(List<Card> cards)
    {
        var pair = cards.GroupBy(c => c.value)
                        .Where(g => g.Count() == 2)
                        .SelectMany(g => g)
                        .ToList();
        return pair.Count == 2 ? pair : null;
    }
}
