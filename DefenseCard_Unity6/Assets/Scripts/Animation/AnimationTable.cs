using System;
using System.Collections;
using UnityEngine;

public class AnimationTable : MonoBehaviour
{
    private RectTransform cardTableUI;
    private bool isAnimating = false; // ���� �ִϸ��̼� ���� ����

    /// <summary>
    /// �ʱ�ȭ
    /// </summary>
    public void Initialize(RectTransform targetUI)
    {
        cardTableUI = targetUI;
    }

    /// <summary>
    /// �ִϸ��̼� ���� Ȯ��
    /// </summary>
    public bool IsAnimating()
    {
        return isAnimating;
    }

    /// <summary>
    /// 1�� + 2�� �ִϸ��̼� ����
    /// </summary>
    public void StartFullAnimation(float offsetDistance, float offsetDuration, float targetY, float targetDuration, Action onComplete)
    {
        if (isAnimating)
            return;

        isAnimating = true;

        StartCoroutine(MoveWithOffset(offsetDistance, offsetDuration, targetY, targetDuration, onComplete));
    }

    /// <summary>
    /// 1�� �̵� �� 2�� �̵� ����
    /// </summary>
    private IEnumerator MoveWithOffset(float offsetDistance, float offsetDuration, float targetY, float targetDuration, Action onComplete)
    {
        // 1�� �̵�
        float initialY = cardTableUI.anchoredPosition.y;
        float offsetTargetY = initialY + offsetDistance;

        //Debug.Log($"[1�� �̵� ����] �ʱ� Y: {initialY}, ��ǥ Y: {offsetTargetY}, �ð�: {offsetDuration}");
        yield return AnimateToY(offsetTargetY, offsetDuration, () =>
        {
            //Debug.Log($"[1�� �̵� �Ϸ�] ���� ��ǥ: {cardTableUI.anchoredPosition.y}");
        });

        // 2�� �̵�
        //Debug.Log($"[2�� �̵� ����] ��ǥ Y: {targetY}, �ð�: {targetDuration}");
        yield return AnimateToY(targetY, targetDuration, () =>
        {
            isAnimating = false;
            //Debug.Log($"[2�� �̵� ����] ���� ���� ��ǥ: {cardTableUI.anchoredPosition.y}");
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// Y ��ǥ�� �̵�
    /// </summary>
    private IEnumerator AnimateToY(float targetY, float duration, Action onComplete)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = cardTableUI.anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, targetY);

        //Debug.Log($"[Y �̵� ����] ���� ��ǥ: {startPosition.y}, ��ǥ ��ǥ: {targetY}, �ð�: {duration}");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            cardTableUI.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cardTableUI.anchoredPosition = targetPosition;
        //Debug.Log($"[Y �̵� �Ϸ�] ���� ��ǥ: {cardTableUI.anchoredPosition.y}");
        onComplete?.Invoke();
    }
}
