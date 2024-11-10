using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class RandomizeJson : MonoBehaviour
{
    private const string JsonFilePath = ResourceDefine.CurrontJsonData;
    private readonly List<string> types = new List<string> { "spades", "hearts", "clubs", "diamonds" };
    private readonly List<int> values = Enumerable.Range(1, 13).ToList(); // 1~13�� �� (A~K)

    [SerializeField] private List<Card> cardDeck; // Inspector â�� ī�� ��� ǥ��

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

    private void Start()
    {
        LoadJsonData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RandomizeCards();
            SaveJsonData();
            Debug.Log("JSON ������ �������� ����Ǿ����ϴ�.");
        }
    }

    private void LoadJsonData()
    {
        if (File.Exists(JsonFilePath))
        {
            string jsonContent = File.ReadAllText(JsonFilePath);
            CardCollection cardCollection = JsonUtility.FromJson<CardCollection>(jsonContent);
            cardDeck = cardCollection.cards;
            Debug.Log("JSON ���� �ε� ����");
        }
        else
        {
            Debug.LogError("JSON ������ ã�� �� �����ϴ�.");
        }
    }

    private void RandomizeCards()
    {
        HashSet<string> uniqueCombinations = new HashSet<string>();

        foreach (var card in cardDeck)
        {
            string newCombination;
            do
            {
                card.type = types[Random.Range(0, types.Count)];
                card.value = values[Random.Range(0, values.Count)];
                newCombination = $"{card.type}-{card.value}";
            }
            while (!uniqueCombinations.Add(newCombination)); // �ߺ����� �ʴ� ������ ������ ������ �ݺ�
        }
    }

    private void SaveJsonData()
    {
        CardCollection cardCollection = new CardCollection { cards = cardDeck };
        string jsonContent = JsonUtility.ToJson(cardCollection, true);
        File.WriteAllText(JsonFilePath, jsonContent);
    }
}
