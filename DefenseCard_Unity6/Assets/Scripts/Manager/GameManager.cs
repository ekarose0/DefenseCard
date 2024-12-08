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

    public bool canInputKey = true; // 키입력 가능 여부
    public bool isSpaceActionAvailable = true; // Space 연출 가능 여부

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
            Debug.LogError("연출이 종료되지 않았습니다");
    }

    public void UsedBinAction()
    {
        if (canInputKey)
        {
            // CardManager의 myHand가 5개의 데이터를 가지고 있는지 확인
            if (CardManager.myHand != null && CardManager.myHand.Count == 5)
            {
                // NoticeUI 업데이트
                UIManager.UpdateNoticeText("Press 'Space' to Setting Hand or 'Tab' to Disable UI!");

                // CardManager에서 현재 존재하는 카드 5장 삭제
                CardManager.ClearCurrentCards();

                // curRound 증가
                CardManager.IncrementRound();

                // 딜레이 후 족보 이름 업데이트
                StartCoroutine(UpdateBestHandWithDelay());

                isSpaceActionAvailable = true; // Space 연출 다시 활성화
            }
            else
                Debug.LogError("현재 손에 카드가 5장이 없습니다.");
        }
        else
            Debug.LogError("연출이 종료되지 않았습니다");
    }

    public void NowCardUpdate()
    {
        // 딜레이 후 족보 이름 업데이트
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
