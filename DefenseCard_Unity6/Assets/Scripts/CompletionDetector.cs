using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class CompletionDetector : MonoBehaviour
{
    private const string JsonFilePath = ResourceDefine.CurrontJsonData; // JSON ���� ���
    private List<Card> cardDeck; // JSON���� �ҷ��� ī�� ����Ʈ
    [SerializeField] private string bestHandDescription; // Inspector�� ���� ���� ���� ��Ī�� ǥ��
    private System.DateTime lastModifiedTime; // JSON ������ ������ ���� �ð� ���

    // ī�� �����͸� ��Ÿ���� Ŭ����
    [System.Serializable]
    public class Card
    {
        public int id; // ī���� ���� ID
        public string type; // ī���� Ÿ�� (spades, hearts, clubs, diamonds)
        public int value; // ī���� �� (1~13)
    }

    // JSON ������ ������ ���� ī�� �÷��� Ŭ����
    [System.Serializable]
    public class CardCollection
    {
        public List<Card> cards;
    }

    private void Start()
    {
        // JSON �����͸� ó�� �ε��ϰ� ���� ���� ������ Inspector�� ǥ��
        LoadJsonData();
        bestHandDescription = EvaluateBestPokerHand(cardDeck);
    }

    private void Update()
    {
        // ���� ���� ���� - �� �����Ӹ��� ������ ������ ���� �ð��� Ȯ��
        if (File.Exists(JsonFilePath))
        {
            var currentModifiedTime = File.GetLastWriteTime(JsonFilePath);
            if (currentModifiedTime != lastModifiedTime)
            {
                lastModifiedTime = currentModifiedTime;
                LoadJsonData(); // ����� ��� JSON �����͸� �ٽ� �ε�
                bestHandDescription = EvaluateBestPokerHand(cardDeck); // ���� ����
                Debug.Log("JSON ������ ����Ǿ� �ٽ� �ε�Ǿ����ϴ�.");
            }
        }
    }

    // JSON ������ �ε��Ͽ� ī�� �����͸� �ʱ�ȭ�ϴ� �޼���
    private void LoadJsonData()
    {
        if (File.Exists(JsonFilePath))
        {
            string jsonContent = File.ReadAllText(JsonFilePath);
            CardCollection cardCollection = JsonUtility.FromJson<CardCollection>(jsonContent);
            cardDeck = cardCollection.cards;
            lastModifiedTime = File.GetLastWriteTime(JsonFilePath); // ������ ���� �ð� ������Ʈ
            Debug.Log("JSON ���� �ε� ����");
        }
        else
        {
            Debug.LogError("JSON ������ ã�� �� �����ϴ�.");
        }
    }

    // ���� ���� ��Ŀ ������ ���ϴ� �޼���
    private string EvaluateBestPokerHand(List<Card> cards)
    {
        // �� ������ ���Ͽ� ���� ���� ������ �̸��� ��ȯ
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

    // �� ������ ���ϴ� �޼����

    // Straight Flush: ���� Ÿ���� ���ӵ� 5�� Ž��
    private List<Card> FindStraightFlush(List<Card> cards)
    {
        var flushCards = cards.GroupBy(c => c.type)
                              .Where(g => g.Count() >= 5)
                              .SelectMany(g => g)
                              .ToList();
        if (flushCards.Count < 5) return null;
        return FindStraight(flushCards);
    }

    // Four of a Kind: ���� ���� ī�� 4�� Ž��
    private List<Card> FindFourOfAKind(List<Card> cards)
    {
        var fourOfAKind = cards.GroupBy(c => c.value)
                               .Where(g => g.Count() == 4)
                               .SelectMany(g => g)
                               .ToList();
        return fourOfAKind.Count == 4 ? fourOfAKind : null;
    }

    // Full House: ���� ���� ī�� 3�� + 2�� Ž��
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

    // Flush: ���� Ÿ���� ī�� 5�� Ž��
    private List<Card> FindFlush(List<Card> cards)
    {
        var flush = cards.GroupBy(c => c.type)
                         .Where(g => g.Count() >= 5)
                         .SelectMany(g => g.Take(5))
                         .ToList();
        return flush.Count == 5 ? flush : null;
    }

    // Straight: ���ӵ� ���� ī�� 5�� Ž��
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

    // ī�尡 ���ӵ� ���� �������� Ȯ��
    private bool IsSequential(List<Card> cards)
    {
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i + 1].value != cards[i].value + 1) return false;
        }
        return true;
    }

    // Three of a Kind: ���� ���� ī�� 3�� Ž��
    private List<Card> FindThreeOfAKind(List<Card> cards)
    {
        var threeOfAKind = cards.GroupBy(c => c.value)
                                .Where(g => g.Count() == 3)
                                .SelectMany(g => g)
                                .ToList();
        return threeOfAKind.Count == 3 ? threeOfAKind : null;
    }

    // Two Pair: 2���� ��� Ž��
    private List<Card> FindTwoPair(List<Card> cards)
    {
        var pairs = cards.GroupBy(c => c.value)
                         .Where(g => g.Count() == 2)
                         .Take(2)
                         .SelectMany(g => g)
                         .ToList();
        return pairs.Count == 4 ? pairs : null;
    }

    // One Pair: ���� ���� ī�� 2�� Ž��
    private List<Card> FindOnePair(List<Card> cards)
    {
        var pair = cards.GroupBy(c => c.value)
                        .Where(g => g.Count() == 2)
                        .SelectMany(g => g)
                        .ToList();
        return pair.Count == 2 ? pair : null;
    }
}
