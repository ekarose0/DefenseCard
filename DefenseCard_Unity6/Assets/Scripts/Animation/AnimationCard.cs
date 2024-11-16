using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCard : MonoBehaviour
{
    [Header("������")]
    [SerializeField] private GameObject cardBackPackPrefab; // ī�� ���� ������
    [SerializeField] private GameObject singleCardBackPrefab; // �޸� ī�� ������

    [Header("�ִϸ��̼� ����")]
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float cardDeleteDuration = 0.5f;

    private GameObject cardBackPack;
    private List<int> replacedCardIndices = new List<int>();

    private void Awake()
    {
        cardBackPackPrefab = Resources.Load<GameObject>(ResourceDefine.CardBackPack);
        singleCardBackPrefab = Resources.Load<GameObject>(ResourceDefine.SingleCardBack);
    }

    public void InstantiateCardPack()
    {
        if (cardBackPack == null)
        {
            cardBackPack = Instantiate(cardBackPackPrefab, GameManager.Instance.BackCardPack_Transform);
            RectTransform packRect = cardBackPack.GetComponent<RectTransform>();
            packRect.anchoredPosition = new Vector2(-1100, 250);
        }
    }

    public void SetReplacedCards(List<int> indices)
    {
        replacedCardIndices = indices;
    }

    public void PlayCardDrawAnimation(bool useFullHand)
    {
        StartCoroutine(CardDrawSequence(useFullHand));
    }

    public void PlayCardReplaceAnimation(List<GameObject> cardsToDelete)
    {
        StartCoroutine(CardReplaceSequence(cardsToDelete));
    }

    private IEnumerator CardDrawSequence(bool useFullHand)
    {
        // 0�ܰ�: ī�� ���� ��Ȱ��ȭ
        GameManager.Instance.CardManager.SetCardSelection(false);

        // 1�ܰ�: ī���� �̵�
        RectTransform packRect = cardBackPack.GetComponent<RectTransform>();
        yield return MoveToPosition(packRect, new Vector2(-790, 250), moveDuration);

        // 2�ܰ�: �޸� ī�� ���� �� �̵�
        int cardCount = useFullHand ? 5 : replacedCardIndices.Count;
        GameObject[] backCards = new GameObject[cardCount];

        for (int i = 0; i < cardCount; i++)
        {
            int index = useFullHand ? i : replacedCardIndices[i];

            backCards[i] = Instantiate(singleCardBackPrefab, GameManager.Instance.SingleBackCard_Transform);
            RectTransform cardRect = backCards[i].GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2(-790, 250);

            float targetX = ((RectTransform)GameManager.Instance.CardManager.cardPositions[index]).anchoredPosition.x;
            StartCoroutine(MoveToPosition(cardRect, new Vector2(targetX, 250), moveDuration));
        }

        yield return new WaitForSeconds(moveDuration);

        // 3�ܰ�: ī�� �Ʒ��� �̵�
        foreach (var backCard in backCards)
        {
            if (backCard)
            {
                RectTransform rect = backCard.GetComponent<RectTransform>();
                StartCoroutine(MoveToPosition(rect, new Vector2(rect.anchoredPosition.x, 2), moveDuration));
            }
        }

        yield return new WaitForSeconds(moveDuration);

        // 4�ܰ�: ī�� ����
        if (useFullHand)
        {
            GameManager.Instance.CardManager.InitializeStartingHand();
        }
        else
        {
            GameManager.Instance.CardManager.GenerateReplacedCards(replacedCardIndices);
        }

        // �޸� ī�� ���� �� ���� ����
        StartCoroutine(FinishDrawSequence(backCards, packRect));
    }

    private IEnumerator CardReplaceSequence(List<GameObject> cardsToDelete)
    {
        // ī�� ����
        yield return DeleteCardsWithFade(cardsToDelete);

        // ī�� ���� ����
        yield return CardDrawSequence(false);
    }

    private IEnumerator FinishDrawSequence(GameObject[] backCards, RectTransform packRect)
    {
        // 5�ܰ�: �޸� ī�� X�� �̵�
        foreach (var backCard in backCards)
        {
            if (backCard)
            {
                RectTransform rect = backCard.GetComponent<RectTransform>();
                StartCoroutine(MoveToPosition(rect, new Vector2(rect.anchoredPosition.x - 10, rect.anchoredPosition.y), 0.2f));
            }
        }

        yield return new WaitForSeconds(0.2f);

        // 6�ܰ�: �ո� ī�带 �̵�
        GameManager.Instance.CardManager.MoveFrontCardsToLast();

        // 7�ܰ�: �޸� ī�� X���� �ٽ� +10���� �̵�
        foreach (var backCard in backCards)
        {
            if (backCard)
            {
                RectTransform rect = backCard.GetComponent<RectTransform>();
                StartCoroutine(MoveToPosition(rect, new Vector2(rect.anchoredPosition.x + 10, rect.anchoredPosition.y), 0.2f));
            }
        }

        yield return new WaitForSeconds(0.2f);

        // 8�ܰ�: ī���� ����
        yield return MoveToPosition(packRect, new Vector2(-1100, 250), moveDuration);

        // �޸� ī�� ����
        foreach (var backCard in backCards)
        {
            if (backCard)
                Destroy(backCard);
        }

        // 9�ܰ�: ī���� ���� ���� ���� Ȱ��ȭ �� NowCardUI����
        GameManager.Instance.AnimeManager.CardOnMoveComplete(true);
    }

    private IEnumerator DeleteCardsWithFade(List<GameObject> cardsToDelete)
    {
        float elapsedTime = 0f;

        while (elapsedTime < cardDeleteDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / cardDeleteDuration);

            foreach (var card in cardsToDelete)
            {
                if (card)
                {
                    CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>() ?? card.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = alpha;
                }
            }

            yield return null;
        }

        foreach (var card in cardsToDelete)
        {
            if (card)
                Destroy(card);
        }
    }

    private IEnumerator MoveToPosition(Transform transform, Vector2 targetPosition, float duration)
    {
        RectTransform rectTransform = transform as RectTransform;

        if (rectTransform == null)
        {
            Debug.LogError("Transform is not a RectTransform!");
            yield break;
        }

        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
}
