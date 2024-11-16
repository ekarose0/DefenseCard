using System.Collections.Generic;
using UnityEngine;

public class AnimeManager : MonoBehaviour
{
    [Header("카드 설정")]
    private AnimationCard animationCard; // AnimationCard 컴포넌트

    [Header("애니메이션 테이블 설정")]
    [SerializeField] private AnimationTable animationTable; // AnimationTable 컴포넌트
    private RectTransform cardTableUI; // UIManager에서 가져오는 cardTableUI

    private bool isMovingUp = false;  // 현재 이동 방향 (true: 위로, false: 아래로)
    private bool isAnimating = false; // 애니메이션 진행 상태

    [Header("애니메이션 설정")]
    [SerializeField] private float firstAnimationDuration = 0.5f; // 1차 이동 시간
    [SerializeField] private float firstAnimationDistance = 50f;  // 1차 이동 거리 (상대값)
    [SerializeField] private float secondAnimationDuration = 0.6f;  // 2차 이동 시간
    [SerializeField] private float upperPositionY = -75f;         // 2차 목표 상단 위치
    [SerializeField] private float lowerPositionY = -885f;       // 2차 목표 하단 위치

    private void Awake()
    {
        // 자식 오브젝트에서 AnimationCard와 AnimationTable 컴포넌트 찾기
        animationCard = GetComponentInChildren<AnimationCard>();
        animationTable = GetComponentInChildren<AnimationTable>();

        if (animationCard == null)
        {
            Debug.LogError("AnimationCard를 찾을 수 없습니다. AnimeManager의 자식에 추가되었는지 확인하세요.");
        }

        // AnimationTable 초기화
        if (animationTable == null)
        {
            Debug.LogError("[AnimeManager] AnimationTable이 설정되지 않았습니다.");
            return;
        }

        // UIManager에서 cardTableUI 가져오기
        cardTableUI = GameManager.Instance.UIManager.GetCardTableUI();
        if (cardTableUI == null)
        {
            Debug.LogError("[AnimeManager] CardTableUI를 UIManager에서 가져오지 못했습니다.");
        }
        else
        {
            animationTable.Initialize(cardTableUI);
        }
    }

    private void Start()
    {
        animationCard.InstantiateCardPack(); // 뒷면 카드팩 초기화
    }

    #region Card Anime
    /// <summary>
    /// 초기 5장의 카드 생성 애니메이션 실행
    /// </summary>
    public void StartInitialCardAnimation()
    {
        if (GameManager.Instance.canInputKey)
        {
            if (GameManager.Instance.isSpaceActionAvailable)
            {
                if (!isMovingUp)
                {
                    // 키입력 불가상태 변환
                    GameManager.Instance.canInputKey = false;
                    GameManager.Instance.isSpaceActionAvailable = false;

                    // 작동시작
                    GameManager.Instance.UIManager.UpdateNoticeText("In Progress..."); // NowCardUI 텍스트 출력
                    animationCard.SetReplacedCards(new List<int> { 0, 1, 2, 3, 4 }); // 5장의 카드 생성
                    animationCard.PlayCardDrawAnimation(true); // 전체 카드 연출
                }
                else
                    Debug.LogError("테이블을 올려주세요");
            }
            else
                Debug.LogError("이미 전개되어있는 카드를 사용해주세요");
        }
        else
            Debug.LogError("연출이 종료되지 않았습니다");    
    }

    /// <summary>
    /// 교체된 카드의 애니메이션 실행
    /// </summary>
    public void StartReplaceAnimation(List<GameObject> cardsToDelete, List<int> replacedCardIndices)
    {
        animationCard.SetReplacedCards(replacedCardIndices);
        animationCard.PlayCardReplaceAnimation(cardsToDelete);
    }

    /// <summary>
    /// 카드 연출 완료
    /// </summary>
    /// <param name="result">카드선택 가능여부</param>
    public void CardOnMoveComplete(bool result)
    {
        GameManager.Instance.CardManager.SaveHandToJson();
        GameManager.Instance.CardManager.SetCardSelection(result);
        GameManager.Instance.NowCardUpdate();
        GameManager.Instance.canInputKey = true; // 재입력 가능
    }

    #endregion

    #region Table Anime
    /// <summary>
    /// 카드 테이블 애니메이션 토글
    /// </summary>
    public void ToggleCardTableAnimation()
    {
        if (GameManager.Instance.canInputKey)
        {
            if (isAnimating)
            {
                GameManager.Instance.UIManager.UpdateNoticeText("Animation in progress. Please wait.");
                Debug.LogWarning("[AnimeManager] 애니메이션 진행 중입니다. 입력이 무시됩니다.");
            }
            else
            {
                isAnimating = true; // 애니메이션 상태 활성화
                GameManager.Instance.UIManager.UpdateNoticeText("Now Moving!");
                StartFullAnimation();
            }
        }
        else
        {
            Debug.LogError("연출이 종료되지 않았습니다");
        }
    }

    /// <summary>
    /// 1차 + 2차 애니메이션 시작
    /// </summary>
    private void StartFullAnimation()
    {
        float firstOffset = isMovingUp ? -firstAnimationDistance : firstAnimationDistance;
        float secondTarget = isMovingUp ? upperPositionY : lowerPositionY;

        //Debug.Log($"[토글] 1차 이동 거리: {firstOffset}, 2차 목표 좌표: {secondTarget}");
        animationTable.StartFullAnimation(
            firstOffset,
            firstAnimationDuration,
            secondTarget,
            secondAnimationDuration,
            isMovingUp ? TableOnMoveUpComplete : TableOnMoveDownComplete // 콜백 전달
        );

        isMovingUp = !isMovingUp; // 방향 반전
    }

    /// <summary>
    /// 올라가는 연출 완료 시 호출
    /// </summary>
    private void TableOnMoveUpComplete()
    {
        //Debug.Log("[AnimeManager] 올라가는 연출이 완료되었습니다.");
        GameManager.Instance.UIManager.UpdateNoticeText("Press 'Space' to Next Stage or 'Tab' to Disable UI!");
        isAnimating = false; // 애니메이션 상태 해제
        GameManager.Instance.canInputKey = true; // 키 입력 가능상태 변환
    }

    /// <summary>
    /// 내려가는 연출 완료 시 호출
    /// </summary>
    private void TableOnMoveDownComplete()
    {
        //Debug.Log("[AnimeManager] 내려가는 연출이 완료되었습니다.");
        GameManager.Instance.UIManager.UpdateNoticeText("Press 'Tab' to Active UI!");
        isAnimating = false; // 애니메이션 상태 해제
        GameManager.Instance.canInputKey = true; // 키 입력 가능상태 변환
    }
    #endregion
}
