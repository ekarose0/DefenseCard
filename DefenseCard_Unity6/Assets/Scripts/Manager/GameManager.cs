using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AnimeManager AnimeManager;
    public CardManager CardManager;
    public UIManager UIManager;
    public CompletionDetector CompletionDetector;

    public Transform BackCardPack_Transform;
    public Transform SingleBackCard_Transform;
    public Transform SingleFrontCard_LastTransform;

    public bool canInputKey = true; // Ű�Է� ���� ����
    public bool isSpaceActionAvailable = true; // Space ���� ���� ����

    public int money;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMoney(int amount)
    {
        money += amount;
        UIManager.UpdateMoneyText(money);
    }

    public bool CanAfford(int amount)
    {
        return money >= amount;
    }

    public void ReplaceButtonAction()
    {
        if (canInputKey)
        {
            if (money >= 1)
            {
                CardManager.ReplaceCard();
            }
            else
                UIManager.UpdateNoticeText("Not enough Coin!");
        }
        else
            Debug.LogError("������ ������� �ʾҽ��ϴ�");
    }

    public void UsedBinAction()
    {
        if (canInputKey)
        {
            // CardManager�� myHand�� 5���� �����͸� ������ �ִ��� Ȯ��
            if (CardManager.myHand != null && CardManager.myHand.Count == 5)
            {
                // NoticeUI ������Ʈ
                UIManager.UpdateNoticeText("Press 'Space' to Setting Hand or 'Tab' to Disable UI!");

                // CardManager���� ���� �����ϴ� ī�� 5�� ����
                CardManager.ClearCurrentCards();

                // curRound ����
                CardManager.IncrementRound();

                // ������ �� ���� �̸� ������Ʈ
                StartCoroutine(UpdateBestHandWithDelay());

                isSpaceActionAvailable = true; // Space ���� �ٽ� Ȱ��ȭ
            }
            else
                Debug.LogError("���� �տ� ī�尡 5���� �����ϴ�.");
        }
        else
            Debug.LogError("������ ������� �ʾҽ��ϴ�");
    }

    public void NowCardUpdate()
    {
        // ������ �� ���� �̸� ������Ʈ
        StartCoroutine(UpdateCurrentHandWithDelay());
    }

    private IEnumerator UpdateBestHandWithDelay()
    {
        yield return new WaitForSeconds(0.05f);
        string bestHand = CompletionDetector.GetBestHandDescription();
        UIManager.UpdateMakeCardText($"Best Hand: {bestHand}");
    }

    private IEnumerator UpdateCurrentHandWithDelay()
    {
        yield return new WaitForSeconds(0.05f);
        string currentHandDescription = CompletionDetector.GetBestHandDescription();
        UIManager.UpdateNoticeText(currentHandDescription);
    }
}
