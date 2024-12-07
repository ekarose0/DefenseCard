using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText; // Money UI
    [SerializeField] private TextMeshProUGUI roundText; // Round UI
    [SerializeField] private TextMeshProUGUI makeCardText; // MakeCard UI
    [SerializeField] private RectTransform CardTableUI; // CardTable UI
    [SerializeField] private TextMeshProUGUI noticeText; // Notice UI
    [SerializeField] private Button replaceButton; // Replace 버튼
    [SerializeField] private Button usedButton; // Used 버튼

    private void Awake()
    {
        syncObject();
    }

    /// <summary>
    /// 게임 시작시 각 오브젝트를 동기화 합니다
    /// </summary>
    private void syncObject()
    {
        // ResourceDefine 경로를 통해 UI 요소를 동적으로 찾습니다.
        var moneyUIObject = GameObject.Find(ResourceDefine.MoneyUI);
        if (moneyUIObject != null)
        {
            moneyText = moneyUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"MoneyUI를 찾을 수 없습니다. 경로: {ResourceDefine.MoneyUI}");
        }

        // ResourceDefine 경로를 통해 UI 요소를 동적으로 찾습니다.
        var roundUIObject = GameObject.Find(ResourceDefine.RoundUI);
        if (roundUIObject != null)
        {
            roundText = roundUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"RoundUI를 찾을 수 없습니다. 경로: {ResourceDefine.RoundUI}");
        }

        var makeCardUIObject = GameObject.Find(ResourceDefine.MakeCardUI);
        if (makeCardUIObject != null)
        {
            makeCardText = makeCardUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"MakeCardUI를 찾을 수 없습니다. 경로: {ResourceDefine.MakeCardUI}");
        }

        var CardTableObject = GameObject.Find(ResourceDefine.CardTableUI);
        if (CardTableObject != null)
        {
            CardTableUI = CardTableObject.GetComponentInChildren<RectTransform>();
        }
        else
        {
            Debug.LogError($"NowCardUI를 찾을 수 없습니다. 경로: {ResourceDefine.CardTableUI}");
        }

        var nowCardUIObject = GameObject.Find(ResourceDefine.NoticeUI);
        if (nowCardUIObject != null)
        {
            noticeText = nowCardUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"NowCardUI를 찾을 수 없습니다. 경로: {ResourceDefine.NoticeUI}");
        }

        var replaceButtonObject = GameObject.Find(ResourceDefine.RePlaceButton);
        if (replaceButtonObject != null)
        {
            replaceButton = replaceButtonObject.GetComponent<Button>();
            replaceButton.onClick.AddListener(() => GameManager.Instance.ReplaceButtonAction());
        }
        else
        {
            Debug.LogError($"RePlaceButton을 찾을 수 없습니다. 경로: {ResourceDefine.RePlaceButton}");
        }

        var usedButtonObject = GameObject.Find(ResourceDefine.UsedButton);
        if (usedButtonObject != null)
        {
            usedButton = usedButtonObject.GetComponent<Button>();
            usedButton.onClick.AddListener(() => GameManager.Instance.UsedBinAction());
        }
        else
        {
            Debug.LogError($"UsedButton을 찾을 수 없습니다. 경로: {ResourceDefine.UsedButton}");
        }
    }

    /// <summary>
    /// 게임 시작 시 코인 값을 동기화합니다.
    /// </summary>
    private void Start()
    {
        SyncMoney();
    }

    /// <summary>
    /// MakeCard UI를 업데이트합니다.
    /// </summary>
    /// <param name="text">업데이트할 텍스트</param>
    public void UpdateMakeCardText(string text)
    {
        makeCardText.text = text;
    }

    /// <summary>
    /// Notice UI를 업데이트합니다.
    /// </summary>
    /// <param name="text">업데이트할 텍스트</param>
    public void UpdateNoticeText(string text)
    {
        noticeText.text = text;
    }

    /// <summary>
    /// 현재 코인 값을 GameManager에서 가져와 UI를 초기화합니다.
    /// </summary>
    private void SyncMoney()
    {
        int currentMoney = GameManager.Instance.money; // GameManager에서 현재 코인 값 가져오기
        UpdateMoneyText(currentMoney); // UI 업데이트
    }

    /// <summary>
    /// 코인 텍스트를 업데이트합니다.
    /// </summary>
    /// <param name="money">현재 코인 수량</param>
    public void UpdateMoneyText(int money)
    {
        moneyText.text = $"Coins: {money}";
    }

    /// <summary>
    /// 라운드 텍스트를 업데이트합니다.
    /// </summary>
    /// <param name="round">현재 라운드</param>
    public void UpdateRoundText (int round)
    {
        roundText.text = $"Rounds: {round + 1}";
    }

    /// <summary>
    /// 경고 메시지를 콘솔에 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    public void ShowWarning(string message)
    {
        Debug.LogWarning(message);
    }

    // UIManager 값 찾아주기
    public RectTransform GetCardTableUI()
    {
        return CardTableUI;
    }
}
