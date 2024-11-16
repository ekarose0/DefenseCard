using System.Collections.Generic;
using UnityEngine;

public class AnimeManager : MonoBehaviour
{
    [Header("ī�� ����")]
    private AnimationCard animationCard; // AnimationCard ������Ʈ

    [Header("�ִϸ��̼� ���̺� ����")]
    [SerializeField] private AnimationTable animationTable; // AnimationTable ������Ʈ
    private RectTransform cardTableUI; // UIManager���� �������� cardTableUI

    private bool isMovingUp = false;  // ���� �̵� ���� (true: ����, false: �Ʒ���)
    private bool isAnimating = false; // �ִϸ��̼� ���� ����

    [Header("�ִϸ��̼� ����")]
    [SerializeField] private float firstAnimationDuration = 0.5f; // 1�� �̵� �ð�
    [SerializeField] private float firstAnimationDistance = 50f;  // 1�� �̵� �Ÿ� (��밪)
    [SerializeField] private float secondAnimationDuration = 0.6f;  // 2�� �̵� �ð�
    [SerializeField] private float upperPositionY = -75f;         // 2�� ��ǥ ��� ��ġ
    [SerializeField] private float lowerPositionY = -885f;       // 2�� ��ǥ �ϴ� ��ġ

    private void Awake()
    {
        // �ڽ� ������Ʈ���� AnimationCard�� AnimationTable ������Ʈ ã��
        animationCard = GetComponentInChildren<AnimationCard>();
        animationTable = GetComponentInChildren<AnimationTable>();

        if (animationCard == null)
        {
            Debug.LogError("AnimationCard�� ã�� �� �����ϴ�. AnimeManager�� �ڽĿ� �߰��Ǿ����� Ȯ���ϼ���.");
        }

        // AnimationTable �ʱ�ȭ
        if (animationTable == null)
        {
            Debug.LogError("[AnimeManager] AnimationTable�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // UIManager���� cardTableUI ��������
        cardTableUI = GameManager.Instance.UIManager.GetCardTableUI();
        if (cardTableUI == null)
        {
            Debug.LogError("[AnimeManager] CardTableUI�� UIManager���� �������� ���߽��ϴ�.");
        }
        else
        {
            animationTable.Initialize(cardTableUI);
        }
    }

    private void Start()
    {
        animationCard.InstantiateCardPack(); // �޸� ī���� �ʱ�ȭ
    }

    #region Card Anime
    /// <summary>
    /// �ʱ� 5���� ī�� ���� �ִϸ��̼� ����
    /// </summary>
    public void StartInitialCardAnimation()
    {
        if (GameManager.Instance.canInputKey)
        {
            if (GameManager.Instance.isSpaceActionAvailable)
            {
                if (!isMovingUp)
                {
                    // Ű�Է� �Ұ����� ��ȯ
                    GameManager.Instance.canInputKey = false;
                    GameManager.Instance.isSpaceActionAvailable = false;

                    // �۵�����
                    GameManager.Instance.UIManager.UpdateNoticeText("In Progress..."); // NowCardUI �ؽ�Ʈ ���
                    animationCard.SetReplacedCards(new List<int> { 0, 1, 2, 3, 4 }); // 5���� ī�� ����
                    animationCard.PlayCardDrawAnimation(true); // ��ü ī�� ����
                }
                else
                    Debug.LogError("���̺��� �÷��ּ���");
            }
            else
                Debug.LogError("�̹� �����Ǿ��ִ� ī�带 ������ּ���");
        }
        else
            Debug.LogError("������ ������� �ʾҽ��ϴ�");    
    }

    /// <summary>
    /// ��ü�� ī���� �ִϸ��̼� ����
    /// </summary>
    public void StartReplaceAnimation(List<GameObject> cardsToDelete, List<int> replacedCardIndices)
    {
        animationCard.SetReplacedCards(replacedCardIndices);
        animationCard.PlayCardReplaceAnimation(cardsToDelete);
    }

    /// <summary>
    /// ī�� ���� �Ϸ�
    /// </summary>
    /// <param name="result">ī�弱�� ���ɿ���</param>
    public void CardOnMoveComplete(bool result)
    {
        GameManager.Instance.CardManager.SaveHandToJson();
        GameManager.Instance.CardManager.SetCardSelection(result);
        GameManager.Instance.NowCardUpdate();
        GameManager.Instance.canInputKey = true; // ���Է� ����
    }

    #endregion

    #region Table Anime
    /// <summary>
    /// ī�� ���̺� �ִϸ��̼� ���
    /// </summary>
    public void ToggleCardTableAnimation()
    {
        if (GameManager.Instance.canInputKey)
        {
            if (isAnimating)
            {
                GameManager.Instance.UIManager.UpdateNoticeText("Animation in progress. Please wait.");
                Debug.LogWarning("[AnimeManager] �ִϸ��̼� ���� ���Դϴ�. �Է��� ���õ˴ϴ�.");
            }
            else
            {
                isAnimating = true; // �ִϸ��̼� ���� Ȱ��ȭ
                GameManager.Instance.UIManager.UpdateNoticeText("Now Moving!");
                StartFullAnimation();
            }
        }
        else
        {
            Debug.LogError("������ ������� �ʾҽ��ϴ�");
        }
    }

    /// <summary>
    /// 1�� + 2�� �ִϸ��̼� ����
    /// </summary>
    private void StartFullAnimation()
    {
        float firstOffset = isMovingUp ? -firstAnimationDistance : firstAnimationDistance;
        float secondTarget = isMovingUp ? upperPositionY : lowerPositionY;

        //Debug.Log($"[���] 1�� �̵� �Ÿ�: {firstOffset}, 2�� ��ǥ ��ǥ: {secondTarget}");
        animationTable.StartFullAnimation(
            firstOffset,
            firstAnimationDuration,
            secondTarget,
            secondAnimationDuration,
            isMovingUp ? TableOnMoveUpComplete : TableOnMoveDownComplete // �ݹ� ����
        );

        isMovingUp = !isMovingUp; // ���� ����
    }

    /// <summary>
    /// �ö󰡴� ���� �Ϸ� �� ȣ��
    /// </summary>
    private void TableOnMoveUpComplete()
    {
        //Debug.Log("[AnimeManager] �ö󰡴� ������ �Ϸ�Ǿ����ϴ�.");
        GameManager.Instance.UIManager.UpdateNoticeText("Press 'Space' to Next Stage or 'Tab' to Disable UI!");
        isAnimating = false; // �ִϸ��̼� ���� ����
        GameManager.Instance.canInputKey = true; // Ű �Է� ���ɻ��� ��ȯ
    }

    /// <summary>
    /// �������� ���� �Ϸ� �� ȣ��
    /// </summary>
    private void TableOnMoveDownComplete()
    {
        //Debug.Log("[AnimeManager] �������� ������ �Ϸ�Ǿ����ϴ�.");
        GameManager.Instance.UIManager.UpdateNoticeText("Press 'Tab' to Active UI!");
        isAnimating = false; // �ִϸ��̼� ���� ����
        GameManager.Instance.canInputKey = true; // Ű �Է� ���ɻ��� ��ȯ
    }
    #endregion
}
