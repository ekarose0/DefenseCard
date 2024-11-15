using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThisCardData : MonoBehaviour, IPointerDownHandler
{
    // ī�� Ÿ�԰� �� (private �ʵ�� ����)
    [SerializeField] private CardType _cardType;
    [SerializeField, Range(1, 13)] private int _value;

    // ī���� �ɺ��� ���� ǥ���� UI ���
    public Image symbolImage1;
    public Image symbolImage2;
    public TMP_Text valueText1;
    public TMP_Text valueText2;

    public CardInfo cardInfo;
    public Vector3 originPos;

    public bool isSelected;

    //ī�� ���� �� �̹��� ������Ʈ
    public void Setup(CardInfo _cardInfo)
    {
        cardInfo = _cardInfo;

        _cardType = cardInfo.cardType;
        _value = cardInfo.cardNum;
        isSelected = false;
    }
    private void Start()
    {
        UpdateCardDisplay();
    }
    // ī�� Ÿ�� ������Ƽ: ���� ������ ������ ī���� UI�� ������Ʈ
    public CardType CardType
    {
        get => _cardType;
        set
        {
            _cardType = value;
            UpdateCardDisplay(); // ī�� Ÿ���� ����Ǹ� UI ������Ʈ
        }
    }

    // ī�� �� ������Ƽ: ���� ������ ������ ī���� UI�� ������Ʈ
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateCardDisplay(); // ī�� ���� ����Ǹ� UI ������Ʈ
        }
    }

    // Inspector���� ���� ����� �� �ڵ����� ȣ��Ǵ� �޼���
    private void OnValidate()
    {
        UpdateCardDisplay();
    }

    // ī���� �ɺ��� ���� ������Ʈ�ϴ� �޼���
    private void UpdateCardDisplay()
    {
        // ī�� Ÿ�Կ� ���� �ɺ� �̹��� �̸� ���� (ex: "spade_symbol")
        string spriteName = _cardType.ToString().TrimEnd('s') + "_Symbol";

        // SymbolImage ��ο� �ִ� ��������Ʈ �迭���� �ش� �̸��� ��������Ʈ ã��
        Sprite[] sprites = Resources.LoadAll<Sprite>(ResourceDefine.SymbolImage);
        Sprite symbolSprite = System.Array.Find(sprites, sprite => sprite.name == spriteName);

        // �ɺ� ��������Ʈ�� null�� �ƴϸ� �� ���� �ɺ� �̹����� �Ҵ�
        if (symbolSprite != null)
        {
            symbolImage1.sprite = symbolSprite;
            symbolImage2.sprite = symbolSprite;
        }
        else
        {
            // �ɺ� ��������Ʈ�� ã�� �� ���� ��� ��� �޽��� ���
            Debug.LogWarning($"Symbol image '{spriteName}' could not be found in {ResourceDefine.SymbolImage}.");
        }

        // Value�� ���� �ؽ�Ʈ�� ���� (A, J, Q, K, ����, �Ǵ� ?)
        string displayValue;
        if (_value == 1) displayValue = "A";
        else if (_value == 11) displayValue = "J";
        else if (_value == 12) displayValue = "Q";
        else if (_value == 13) displayValue = "K";
        else if (_value > 1 && _value < 11) displayValue = _value.ToString();
        else displayValue = "?"; // 0 ���� �Ǵ� 13 �ʰ��� ��� "?" ���

        // �� ���� �ؽ�Ʈ UI ��ҿ� ������ ���� ����
        valueText1.text = displayValue;
        valueText2.text = displayValue;

        // ī�� Ÿ�Կ� ���� �ؽ�Ʈ ���� ����
        Color textColor;
        if (_cardType == CardType.clubs || _cardType == CardType.spades)
        {
            textColor = new Color32(43, 41, 41, 255); // ������ �迭
        }
        else
        {
            textColor = new Color32(245, 61, 59, 255); // ������ �迭
        }

        // �� ���� �ؽ�Ʈ UI ��ҿ� ������ ���� ����
        valueText1.color = textColor;
        valueText2.color = textColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!isSelected)
        {
            isSelected = true;
            transform.localPosition = originPos + Vector3.up * 100;
        }
        else
        {
            isSelected = false;
            transform.localPosition = originPos;
        }
    }
}
    /*
    ī�� Ÿ���� �����ϴ¹� (spades, diamonds, hearts, clubs �� �ϳ��� ���� ����)
    myCardData.CardType = CardType.hearts;

    ī���� ���� �����ϴ¹� (1~13 ������ ������ ����)
    myCardData.Value = 11; // ��: 11�� 'J'�� ǥ��
    */