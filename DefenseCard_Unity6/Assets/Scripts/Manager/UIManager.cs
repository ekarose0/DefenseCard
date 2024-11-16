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
    [SerializeField] private Button replaceButton; // Replace ��ư
    [SerializeField] private Button usedButton; // Used ��ư

    private void Awake()
    {
        syncObject();
    }

    /// <summary>
    /// ���� ���۽� �� ������Ʈ�� ����ȭ �մϴ�
    /// </summary>
    private void syncObject()
    {
        // ResourceDefine ��θ� ���� UI ��Ҹ� �������� ã���ϴ�.
        var moneyUIObject = GameObject.Find(ResourceDefine.MoneyUI);
        if (moneyUIObject != null)
        {
            moneyText = moneyUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"MoneyUI�� ã�� �� �����ϴ�. ���: {ResourceDefine.MoneyUI}");
        }

        // ResourceDefine ��θ� ���� UI ��Ҹ� �������� ã���ϴ�.
        var roundUIObject = GameObject.Find(ResourceDefine.RoundUI);
        if (roundUIObject != null)
        {
            roundText = roundUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"RoundUI�� ã�� �� �����ϴ�. ���: {ResourceDefine.RoundUI}");
        }

        var makeCardUIObject = GameObject.Find(ResourceDefine.MakeCardUI);
        if (makeCardUIObject != null)
        {
            makeCardText = makeCardUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"MakeCardUI�� ã�� �� �����ϴ�. ���: {ResourceDefine.MakeCardUI}");
        }

        var CardTableObject = GameObject.Find(ResourceDefine.CardTableUI);
        if (CardTableObject != null)
        {
            CardTableUI = CardTableObject.GetComponentInChildren<RectTransform>();
        }
        else
        {
            Debug.LogError($"NowCardUI�� ã�� �� �����ϴ�. ���: {ResourceDefine.CardTableUI}");
        }

        var nowCardUIObject = GameObject.Find(ResourceDefine.NoticeUI);
        if (nowCardUIObject != null)
        {
            noticeText = nowCardUIObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError($"NowCardUI�� ã�� �� �����ϴ�. ���: {ResourceDefine.NoticeUI}");
        }

        var replaceButtonObject = GameObject.Find(ResourceDefine.RePlaceButton);
        if (replaceButtonObject != null)
        {
            replaceButton = replaceButtonObject.GetComponent<Button>();
            replaceButton.onClick.AddListener(() => GameManager.Instance.ReplaceButtonAction());
        }
        else
        {
            Debug.LogError($"RePlaceButton�� ã�� �� �����ϴ�. ���: {ResourceDefine.RePlaceButton}");
        }

        var usedButtonObject = GameObject.Find(ResourceDefine.UsedButton);
        if (usedButtonObject != null)
        {
            usedButton = usedButtonObject.GetComponent<Button>();
            usedButton.onClick.AddListener(() => GameManager.Instance.UsedBinAction());
        }
        else
        {
            Debug.LogError($"UsedButton�� ã�� �� �����ϴ�. ���: {ResourceDefine.UsedButton}");
        }
    }

    /// <summary>
    /// ���� ���� �� ���� ���� ����ȭ�մϴ�.
    /// </summary>
    private void Start()
    {
        SyncMoney();
    }

    /// <summary>
    /// MakeCard UI�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="text">������Ʈ�� �ؽ�Ʈ</param>
    public void UpdateMakeCardText(string text)
    {
        makeCardText.text = text;
    }

    /// <summary>
    /// Notice UI�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="text">������Ʈ�� �ؽ�Ʈ</param>
    public void UpdateNoticeText(string text)
    {
        noticeText.text = text;
    }

    /// <summary>
    /// ���� ���� ���� GameManager���� ������ UI�� �ʱ�ȭ�մϴ�.
    /// </summary>
    private void SyncMoney()
    {
        int currentMoney = GameManager.Instance.money; // GameManager���� ���� ���� �� ��������
        UpdateMoneyText(currentMoney); // UI ������Ʈ
    }

    /// <summary>
    /// ���� �ؽ�Ʈ�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="money">���� ���� ����</param>
    public void UpdateMoneyText(int money)
    {
        moneyText.text = $"Coins: {money}";
    }

    /// <summary>
    /// ���� �ؽ�Ʈ�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="round">���� ����</param>
    public void UpdateRoundText (int round)
    {
        roundText.text = $"Rounds: {round}";
    }

    /// <summary>
    /// ��� �޽����� �ֿܼ� ǥ���մϴ�.
    /// </summary>
    /// <param name="message">ǥ���� �޽���</param>
    public void ShowWarning(string message)
    {
        Debug.LogWarning(message);
    }

    // UIManager �� ã���ֱ�
    public RectTransform GetCardTableUI()
    {
        return CardTableUI;
    }
}
