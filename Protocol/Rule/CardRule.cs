using Protocol.Constants;
using Protocol.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Rule
{
    public class CardRule
    {
        #region 卡牌操作
        public static void CardSort(List<CardDto> cards, int sortType = -1)
        {
            cards.Sort((a, b) =>
            {
                return a.Id.CompareTo(b.Id) * sortType;
            });
        }

        /// <summary>
        /// 判断是否能够出牌
        /// </summary>
        /// <param name="preCards"></param>
        /// <param name="curCards"></param>
        /// <returns></returns>
        public static bool CardsCompare(List<CardDto> preCards, List<CardDto> curCards)
        {
            CardSort(preCards);
            CardSort(curCards);

            int preType = GetCardType(preCards);
            int curType = GetCardType(curCards);


            if (curType == CardType.NONE)
            {
                return false;
            }

            if (curType == CardType.JOKE_BOOM)
            {
                return true;
            }
            else if (curType == CardType.BOOM) // 出炸弹判断
            {
                switch (preType)
                {
                    case CardType.BOOM: return curCards[0].Weight > preCards[0].Weight;
                    case CardType.JOKE_BOOM: return false;
                    default: return true;
                }
            }
            else
            {
                if (curType != preType || preCards.Count != curCards.Count)
                {
                    return false;
                }

                return SameTypeCompare(preCards, curCards);
            }
        }
        #endregion

        #region 获取卡牌数据
        public static int GetCardType(List<CardDto> cards)
        {
            int res = CardType.NONE;
            if (cards != null && cards.Count > 0)
            {
                if (cards.Count > 0)
                {
                    if (IsSingle(cards))
                    {
                        res = CardType.SINGLE;
                    }
                    else if (IsDouble(cards))
                    {
                        res = CardType.DOUBLE;
                    }
                    else if (IsStraight(cards))
                    {
                        res = CardType.STRAIGHT;
                    }
                    else if (IsDoubleStraight(cards))
                    {
                        res = CardType.DOUBLE_STRAIGHT;
                    }
                    else if (IsTribleStraight(cards))
                    {
                        res = CardType.TRIBLE_STRAIGHT;
                    }
                    else if (IsThree(cards))
                    {
                        res = CardType.THREE;
                    }
                    else if (IsThreeOne(cards))
                    {
                        res = CardType.THREE_ONE;
                    }
                    else if (IsThreeTow(cards))
                    {
                        res = CardType.THREE_TOW;
                    }
                    else if (IsBoom(cards))
                    {
                        res = CardType.BOOM;
                    }
                    else if (IsJokeBoom(cards))
                    {
                        res = CardType.JOKE_BOOM;
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// 获取三带一 或 三带二的主牌大小
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static int GetThreeCardWeight(List<CardDto> cards)
        {
            int type = GetCardType(cards);
            switch (type)
            {
                case CardType.THREE: return cards[0].Weight;
                case CardType.THREE_ONE:
                    if (IsThree(cards.GetRange(0, 3))) { return cards[0].Weight; }
                    else { return cards[3].Weight; }
                case CardType.THREE_TOW:
                    if (IsThree(cards.GetRange(0, 3))) { return cards[0].Weight; }
                    else { return cards[4].Weight; }
                default: return -1;
            }
        }
        #endregion


        #region 牌型判断
        public static bool IsSingle(List<CardDto> cards)
        {
            return cards.Count == 1;
        }

        public static bool IsDouble(List<CardDto> cards)
        {
            return cards.Count == 2 && cards[0].Weight == cards[1].Weight;
        }

        public static bool IsStraight(List<CardDto> cards)
        {
            if (cards != null && cards.Count >= 5)
            {
                int tmpWeight = cards[0].Weight;
                for (int i = 1; i < cards.Count; i++)
                {
                    if (cards[i].Weight - i != tmpWeight) { return false; }
                }
                return true;
            }

            return false;
        }

        public static bool IsDoubleStraight(List<CardDto> cards)
        {
            int len = cards != null ? cards.Count : 0;
            if (len >= 4 && len % 2 == 0)
            {
                for (int i = 1; i < len; i++)
                {
                    int preWeight = cards[i - 1].Weight;
                    int curWeight = cards[i].Weight;
                    // 判断与前一个对是否相差1
                    if (i % 2 == 0)
                    {
                        if (curWeight - preWeight != 1)
                        {
                            return false;
                        }
                    }
                    else // 判断是否为相同牌
                    {
                        if (curWeight != preWeight)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsTribleStraight(List<CardDto> cards)
        {
            int len = cards != null ? cards.Count : 0;
            if (len >= 6 && len % 3 == 0)
            {
                // 判断三牌是否相同
                for (int i = 2; i < len; i += 3)
                {
                    int preWeight1 = cards[i - 1].Weight;
                    int preWeight2 = cards[i - 2].Weight;
                    int curWeight = cards[i].Weight;
                    if (preWeight1 != preWeight2 || curWeight != preWeight1)
                    {
                        return false;
                    }
                }

                // 判断是否连贯
                for (int i = 3; i < len; i += 3)
                {
                    int preWeight = cards[i - 1].Weight;
                    int curWeight = cards[i].Weight;
                    if (curWeight - preWeight != 1)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool IsThree(List<CardDto> cards)
        {
            return cards != null && cards.Count == 3 && cards[0].Weight == cards[1].Weight && cards[0].Weight == cards[2].Weight;
        }

        public static bool IsThreeOne(List<CardDto> cards)
        {
            if (cards != null && cards.Count == 4)
            {
                int firstWeight = cards[0].Weight;
                int lastWeight = cards[3].Weight;
                return IsThree(cards.GetRange(0, 3)) || IsThree(cards.GetRange(1, 3));
            }

            return false;
        }

        public static bool IsThreeTow(List<CardDto> cards)
        {
            if (cards != null && cards.Count == 5)
            {
                return IsDouble(cards.GetRange(0, 2)) && IsThree(cards.GetRange(2, 3)) || IsThree(cards.GetRange(0, 3)) && IsDouble(cards.GetRange(3, 2));
            }

            return false;
        }

        public static bool IsBoom(List<CardDto> cards)
        {
            if (cards != null && cards.Count == 4)
            {
                int tmpWeight = cards[0].Weight;
                return tmpWeight == cards[1].Weight && tmpWeight == cards[2].Weight && tmpWeight == cards[3].Weight;
            }

            return false;
        }

        public static bool IsJokeBoom(List<CardDto> cards)
        {
            if (cards != null && cards.Count == 2)
            {
                return cards[0].Weight > CardWeight.TWO && cards[1].Weight > CardWeight.TWO;
            }

            return false;
        }

        public static bool SameTypeCompare(List<CardDto> preCards, List<CardDto> curCards)
        {
            CardSort(preCards);
            CardSort(curCards);

            int preType = GetCardType(preCards);
            int curType = GetCardType(curCards);

            if (preType != curType) { return false; }

            switch (curType)
            {
                case CardType.THREE_ONE:
                case CardType.THREE_TOW:
                    return GetThreeCardWeight(curCards) > GetThreeCardWeight(preCards);
                default:
                    return curCards.Last().Weight > preCards.Last().Weight;

            }
        }
        #endregion
    }
}
