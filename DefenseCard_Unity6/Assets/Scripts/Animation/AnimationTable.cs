using System;
using System.Collections;
using UnityEngine;

public class AnimationTable : MonoBehaviour
{
    private RectTransform cardTableUI;
    private bool isAnimating = false; // 현재 애니메이션 진행 여부

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize(RectTransform targetUI)
    {
        cardTableUI = targetUI;
    }

    /// <summary>
    /// 애니메이션 상태 확인
    /// </summary>
    public bool IsAnimating()
    {
        return isAnimating;
    }

    /// <summary>
    /// 1차 + 2차 애니메이션 시작
    /// </summary>
    public void StartFullAnimation(float offsetDistance, float offsetDuration, float targetY, float targetDuration, Action onComplete)
    {
        if (isAnimating)
            return;

        isAnimating = true;

        StartCoroutine(MoveWithOffset(offsetDistance, offsetDuration, targetY, targetDuration, onComplete));
    }

    /// <summary>
    /// 1차 이동 후 2차 이동 수행
    /// </summary>
    private IEnumerator MoveWithOffset(float offsetDistance, float offsetDuration, float targetY, float targetDuration, Action onComplete)
    {
        // 1차 이동
        float initialY = cardTableUI.anchoredPosition.y;
        float offsetTargetY = initialY + offsetDistance;

        //Debug.Log($"[1차 이동 시작] 초기 Y: {initialY}, 목표 Y: {offsetTargetY}, 시간: {offsetDuration}");
        yield return AnimateToY(offsetTargetY, offsetDuration, () =>
        {
            //Debug.Log($"[1차 이동 완료] 도달 좌표: {cardTableUI.anchoredPosition.y}");
        });

        // 2차 이동
        //Debug.Log($"[2차 이동 시작] 목표 Y: {targetY}, 시간: {targetDuration}");
        yield return AnimateToY(targetY, targetDuration, () =>
        {
            isAnimating = false;
            //Debug.Log($"[2차 이동 종료] 최종 도달 좌표: {cardTableUI.anchoredPosition.y}");
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// Y 좌표로 이동
    /// </summary>
    private IEnumerator AnimateToY(float targetY, float duration, Action onComplete)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = cardTableUI.anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, targetY);

        //Debug.Log($"[Y 이동 시작] 시작 좌표: {startPosition.y}, 목표 좌표: {targetY}, 시간: {duration}");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            cardTableUI.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cardTableUI.anchoredPosition = targetPosition;
        //Debug.Log($"[Y 이동 완료] 현재 좌표: {cardTableUI.anchoredPosition.y}");
        onComplete?.Invoke();
    }
}
