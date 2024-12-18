using UnityEngine;

public enum CardType 
{   spades, 
    diamonds, 
    hearts, 
    clubs
}

[System.Serializable]
public class CardInfo
{
    public CardType cardType;
    public int cardNum;
}

[CreateAssetMenu(fileName = "Cards", menuName = "ScriptableObjects/CardsSO")]
public class CardSO : ScriptableObject
{
    public CardInfo[] cardInfo;
}
