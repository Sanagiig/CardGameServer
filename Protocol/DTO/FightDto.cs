using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    public enum Identity
    {
        Farmer, Landlord
    }
    public class FightDto
    {

    }

    [Serializable]
    public class CardDto
    {
        public int Id;
        public string Name;
        public int Color;
        public int Weight;

        public CardDto() { }

        public CardDto(int id, string name, int color, int weight)
        {
            Id = id;
            Name = name;
            Color = color;
            Weight = weight;
        }
    }

    [Serializable]
    public class PutCardDto
    {
        public int AccountID;
        public int CardType;
        public List<CardDto> Cards;
        public PutCardDto() { }
        public PutCardDto(int id, int type, List<CardDto> cards)
        {
            AccountID = id;
            CardType = type;
            Cards = cards;
        }
    }

    [Serializable]
    public class UpdateCardsDto
    {
        public List<CardDto> SelfCardList;
        public int LeftCardRemainCount;
        public int RightCardRemainCount;
        public int Multipler;
        public UpdateCardsDto()
        {

        }
        public UpdateCardsDto(List<CardDto> selfCardList, int leftCount, int rightCount, int multipler)
        {
            SelfCardList = selfCardList;
            LeftCardRemainCount = leftCount;
            RightCardRemainCount = rightCount;
            Multipler = multipler;
        }
    }

    [Serializable]
    public class UpdateRemainCardsDto
    {
        public List<CardDto> CardList;
        public UpdateRemainCardsDto() { }
        public UpdateRemainCardsDto(List<CardDto> cardList)
        {
            CardList = cardList;
        }
    }

    [Serializable]
    public class SayDto
    {
        public int Id;
        public int Type;
        public string Txt;

        public SayDto() { }
        public SayDto(int id, int type, string txt)
        {
            Id = id;
            Type = type;
            Txt = txt;
        }
    }

    [Serializable]
    public class SetActionDto
    {
        public int Id;
        public int State;
        public SetActionDto() { }
        public SetActionDto(int id, int state)
        {
            Id = id;
            State = state;
        }
    }

    [Serializable]
    public class SetIdentityDto
    {
        public int Id;
        public int State;
        public SetIdentityDto() { }
        public SetIdentityDto(int id, int state)
        {
            Id = id;
            State = state;

        }
    }

    [Serializable]
    public class GameOverDto
    {
        public int[] IdArr;
        public int[] ScoreArr;
        public GameOverDto()
        {

        }

        public GameOverDto(int[] idArr, int[] scoreArr)
        {
            IdArr = idArr;
            ScoreArr = scoreArr;
        }
    }

    [Serializable]
    public class RoundDto
    {
        public int Multipler;
        public RoundDto()
        {

        }

        public RoundDto(int multipler)
        {
            Multipler = multipler;
        }
    }
}
