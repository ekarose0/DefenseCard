using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ThisCardData : MonoBehaviour
{
    // 카드 타입과 값 (private 필드로 설정)
    [SerializeField] private CardType _cardType;
    [SerializeField, Range(1, 13)] private int _value;

    // 카드의 심볼과 값을 표시할 UI 요소
    public Image symbolImage1;
    public Image symbolImage2;
    public TMP_Text valueText1;
    public TMP_Text valueText2;

    // 카드 타입 프로퍼티: 값이 설정될 때마다 카드의 UI를 업데이트
    public CardType CardType
    {
        get => _cardType;
        set
        {
            _cardType = value;
            UpdateCardDisplay(); // 카드 타입이 변경되면 UI 업데이트
        }
    }

    // 카드 값 프로퍼티: 값이 설정될 때마다 카드의 UI를 업데이트
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateCardDisplay(); // 카드 값이 변경되면 UI 업데이트
        }
    }

    // Start 메서드에서 카드 UI를 초기화
    private void Start()
    {
        UpdateCardDisplay();
    }

    // Inspector에서 값이 변경될 때 자동으로 호출되는 메서드
    private void OnValidate()
    {
        UpdateCardDisplay();
    }

    // 카드의 심볼과 값을 업데이트하는 메서드
    private void UpdateCardDisplay()
    {
        // 카드 타입에 따라 심볼 이미지 이름 생성 (ex: "spade_symbol")
        string spriteName = _cardType.ToString().TrimEnd('s') + "_Symbol";

        // SymbolImage 경로에 있는 스프라이트 배열에서 해당 이름의 스프라이트 찾기
        Sprite[] sprites = Resources.LoadAll<Sprite>(ResourceDefine.SymbolImage);
        Sprite symbolSprite = System.Array.Find(sprites, sprite => sprite.name == spriteName);

        // 심볼 스프라이트가 null이 아니면 두 개의 심볼 이미지에 할당
        if (symbolSprite != null)
        {
            symbolImage1.sprite = symbolSprite;
            symbolImage2.sprite = symbolSprite;
        }
        else
        {
            // 심볼 스프라이트를 찾을 수 없는 경우 경고 메시지 출력
            Debug.LogWarning($"Symbol image '{spriteName}' could not be found in {ResourceDefine.SymbolImage}.");
        }

        // Value에 따라 텍스트를 설정 (A, J, Q, K, 숫자, 또는 ?)
        string displayValue;
        if (_value == 1) displayValue = "A";
        else if (_value == 11) displayValue = "J";
        else if (_value == 12) displayValue = "Q";
        else if (_value == 13) displayValue = "K";
        else if (_value > 1 && _value < 11) displayValue = _value.ToString();
        else displayValue = "?"; // 0 이하 또는 13 초과일 경우 "?" 출력

        // 두 개의 텍스트 UI 요소에 동일한 값을 설정
        valueText1.text = displayValue;
        valueText2.text = displayValue;

        // 카드 타입에 따른 텍스트 색상 설정
        Color textColor;
        if (_cardType == CardType.clubs || _cardType == CardType.spades)
        {
            textColor = new Color32(43, 41, 41, 255); // 검정색 계열
        }
        else
        {
            textColor = new Color32(245, 61, 59, 255); // 빨간색 계열
        }

        // 두 개의 텍스트 UI 요소에 동일한 색상 설정
        valueText1.color = textColor;
        valueText2.color = textColor;
    }
}
    /*
    카드 타입을 변경하는법 (spades, diamonds, hearts, clubs 중 하나로 설정 가능)
    myCardData.CardType = CardType.hearts;

    카드의 값을 변경하는법 (1~13 사이의 값으로 설정)
    myCardData.Value = 11; // 예: 11은 'J'로 표시
    */