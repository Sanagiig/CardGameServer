using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Constants
{
    public class CardColor
    {
        // 方块
        public const int NONE = 0;
        public const int SQUARE = 1;
        public const int CLUB = 2;
        public const int HEART = 3;
        public const int SPADE = 4;

        public static string GetColorName(int color)
        {
            switch (color)
            {
                case NONE: return "";
                case SQUARE: return "Square";
                case CLUB: return "Club";
                case HEART: return "Heart";
                case SPADE: return "Spade";
                default:
                    throw new Exception("Unkonw Color.");
            }
        }
    }

    public class CardWeight
    {
        public const int THREE = 3;
        public const int FOUR = 4;
        public const int FIVE = 5;
        public const int SIX = 6;
        public const int SEVEN = 7;
        public const int EIGHT = 8;
        public const int NINE = 9;
        public const int TEN = 10;
        public const int JACK = 11;
        public const int QUEEN = 12;
        public const int KING = 13;
        public const int ONE = 14;
        public const int TWO = 15;
        public const int SJOKE = 100;
        public const int LJOKE = 101;

        public static string GetWeightName(int weight)
        {
            switch (weight)
            {
                case THREE:
                    return "Three";
                case FOUR:
                    return "Four";
                case FIVE:
                    return "Five";
                case SIX:
                    return "Six";
                case SEVEN:
                    return "Seven";
                case EIGHT:
                    return "Eight";
                case NINE:
                    return "Nine";
                case TEN:
                    return "Ten";
                case JACK:
                    return "Jack";
                case QUEEN:
                    return "Queen";
                case KING:
                    return "King";
                case ONE:
                    return "One";
                case TWO:
                    return "Two";
                case SJOKE:
                    return "SJoker";
                case LJOKE:
                    return "LJoker";
                default:
                    throw new Exception("Unkonw Weight.");
            }
        }
    }

    public class CardType
    {
        public const int NONE = 0;
        public const int SINGLE = 1;
        public const int DOUBLE = 2;
        public const int STRAIGHT = 3; // 顺子 45678
        public const int DOUBLE_STRAIGHT = 4; // 双顺子 44 55 66
        public const int TRIBLE_STRAIGHT = 5; // 三顺子 444 555 666
        public const int THREE = 6; // 三不带 444
        public const int THREE_ONE = 7; // 三带一
        public const int THREE_TOW = 8; // 三带二
        public const int BOOM = 9; // 炸弹
        public const int JOKE_BOOM = 10; // 王炸
    }
}

