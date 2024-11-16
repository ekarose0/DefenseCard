using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThisCardData : MonoBehaviour, IPointerDownHandler
{
    // 카드 타입과 값 (private 필드로 설정)
    [SerializeField] private CardType _cardType;
    [SerializeField, Range(1, 13)] private int _value;

    // 카드의 심볼과 값을 표시할 UI 요소
    public Image symbolImage1;
    public Image symbolImage2;
    public TMP_Text valueText1;
    public TMP_Text valueText2;

    public CardInfo cardInfo; // 카드 정보
    public Vector3 originPos; // 카드 원래 위치
    public bool isSelected;   // 카드 선택 여부
    public bool isSelectionEnabled = false; // 선택 가능 여부

    // 카드 정보 및 이미지 업데이트
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

    // 카드 타입 프로퍼티
    public CardType CardType
    {
        get => _cardType;
        set
        {
            _cardType = value;
            UpdateCardDisplay(); // 카드 타입이 변경되면 UI 업데이트
        }
    }

    // 카드 값 프로퍼티
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateCardDisplay(); // 카드 값이 변경되면 UI 업데이트
        }
    }

    // Inspector에서 값이 변경될 때 자동으로 호출
    private void OnValidate()
    {
        UpdateCardDisplay();
    }

    // 카드의 심볼과 값을 업데이트
    private void UpdateCardDisplay()
    {
        string spriteName = _cardType.ToString().TrimEnd('s') + "_Symbol";

        // SymbolImage 경로에서 해당 이름의 스프라이트 찾기
        Sprite[] sprites = Resources.LoadAll<Sprite>(ResourceDefine.SymbolImage);
        Sprite symbolSprite = System.Array.Find(sprites, sprite => sprite.name == spriteName);

        // 심볼 스프라이트 설정
        if (symbolSprite != null)
        {
            symbolImage1.sprite = symbolSprite;
            symbolImage2.sprite = symbolSprite;
        }
        else
        {
            Debug.LogWarning($"Symbol image '{spriteName}' not found in {ResourceDefine.SymbolImage}.");
        }

        // Value에 따라 텍스트 설정
        string displayValue;
        if (_value == 1) displayValue = "A";
        else if (_value == 11) displayValue = "J";
        else if (_value == 12) displayValue = "Q";
        else if (_value == 13) displayValue = "K";
        else if (_value > 1 && _value < 11) displayValue = _value.ToString();
        else displayValue = "?";

        // 텍스트 UI 요소에 값과 색상 설정
        valueText1.text = displayValue;
        valueText2.text = displayValue;

        // 카드 타입에 따른 텍스트 색상 설정
        Color textColor = (_cardType == CardType.clubs || _cardType == CardType.spades)
            ? new Color32(43, 41, 41, 255) // 검정색
            : new Color32(245, 61, 59, 255); // 빨간색

        valueText1.color = textColor;
        valueText2.color = textColor;
    }

    // 카드 선택 가능 활성화
    public void EnableSelection()
    {
        isSelectionEnabled = true;
        Debug.Log($"Card {cardInfo.cardNum} selection enabled.");
    }

    // 카드 선택 가능 비활성화
    public void DisableSelection()
    {
        isSelectionEnabled = false;
        isSelected = false; // 선택 상태 초기화
        transform.localPosition = originPos; // 원래 위치로 복귀
        Debug.Log($"Card {cardInfo.cardNum} selection disabled.");
    }

    // 클릭 이벤트 처리
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelectionEnabled) return; // 선택 가능 상태가 아니면 무시

        if (!isSelected)
        {
            isSelected = true;
            transform.localPosition = originPos + Vector3.up * 100; // 선택 시 위로 이동
        }
        else
        {
            isSelected = false;
            transform.localPosition = originPos; // 선택 해제 시 원래 위치로 이동
        }
    }
}

/*
카드 타입을 변경하는법 (spades, diamonds, hearts, clubs 중 하나로 설정 가능)
myCardData.CardType = CardType.hearts;

카드의 값을 변경하는법 (1~13 사이의 값으로 설정)
myCardData.Value = 11; // 예: 11은 'J'로 표시
*/