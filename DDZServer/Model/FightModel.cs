using AhpilyServer.Utils.ConcurrentVal;
using DDZServer.Cache;
using Protocol.Constants;
using Protocol.DTO;
using Protocol.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Model
{
    public class FightModel
    {
        private int roomID;
        private int[] playerArr;
        private Queue<CardDto> cardQueue = new Queue<CardDto>();
        private List<CardDto> cardCollection = new List<CardDto>();
        private List<CardDto> remainCardList = new List<CardDto>();
        private SynchronizedCollection<int> readyList = new SynchronizedCollection<int>();
        private Dictionary<int, List<CardDto>> playerCardList = new Dictionary<int, List<CardDto>>();
        public int curAction;
        public int dizhuId;
        public int multipler = 1;
        public Rand rand = new Rand();

        public FightModel(int roomId, int[] memberArr)
        {
            roomID = roomId;
            playerArr = memberArr;
            genCard();
            shuffle();
        }

        #region 生成牌
        private void genCard()
        {
            for (int i = CardColor.SQUARE; i <= CardColor.SPADE; i++)
            {
                for (int j = CardWeight.THREE; j <= CardWeight.TWO; j++)
                {
                    int id = i + j * 10;
                    string name = CardColor.GetColorName(i) + CardWeight.GetWeightName(j);
                    CardDto card = new CardDto(id, name, i, j);
                    cardQueue.Enqueue(card);
                }
            }

            cardQueue.Enqueue(new CardDto(10000, "SJoker", CardColor.NONE, CardWeight.SJOKE));
            cardQueue.Enqueue(new CardDto(10001, "LJoker", CardColor.NONE, CardWeight.LJOKE));
        }

        private void shuffle(int num = 1)
        {
            if (num >= 1)
            {
                List<CardDto> cardList = new List<CardDto>();
                Random random = new Random();
                foreach (CardDto card in cardQueue)
                {
                    int idx = random.Next(0, cardList.Count + 1);
                    cardList.Insert(idx, card);
                }
                cardQueue.Clear();
                foreach (CardDto card in cardList)
                {
                    cardQueue.Enqueue(card);
                }
                shuffle(num - 1);
            }
        }

        private void randomAction()
        {
            if (playerArr.Length > 0)
            {
                int rIdx = new Random().Next(0, playerArr.Length);
                int playerID = playerArr[rIdx];
                curAction = playerID;
                CacheCenter.Instance.FightCache.SetNextActive(roomID, curAction);
            }
            else
            {
                curAction = -1;
            }
        }
        #endregion

        #region 状态
        public bool IsFighting()
        {
            return rand.randType == 1;
        }

        public bool IsDizhuWin()
        {
            int count = GetCardList(dizhuId).Count;
            return count == 0;
        }

        public bool IsAllReady()
        {
            return readyList.Count == playerArr.Length;
        }
        public void AddReady(int id)
        {
            if (playerArr.Contains(id) && !readyList.Contains(id))
            {
                readyList.Add(id);
            }
        }
        public void StartFighting()
        {
            rand.randType = 1;
        }
        public int GetRandCount()
        {
            return rand.count;
        }

        public int[] GetPlayerArr()
        {
            return (int[])playerArr.Clone();
        }
        #endregion

        #region 获取牌
        public void ClearCardList()
        {
            rand.count = 0;
            playerCardList.Clear();
            remainCardList.Clear();
        }

        public void DispatchCard()
        {
            randomAction();
            ClearCardList();
            foreach (int id in playerArr)
            {
                playerCardList.Add(id, new List<CardDto>());
            }

            int i = 0;
            foreach (CardDto card in cardQueue)
            {
                if (i < 3)
                {
                    remainCardList.Add(card);
                }
                else
                {
                    int curPlayerId = playerArr[i % playerArr.Length];
                    playerCardList[curPlayerId].Add(card);
                }
                i++;
            }
        }

        public void ReDispatchCard()
        {
            shuffle();
            DispatchCard();
        }

        public void AddRemainCard(int id)
        {
            if (playerArr.Contains(id))
            {
                playerCardList[id].AddRange(remainCardList);
            }
        }

        public List<CardDto> GetCardList(int id)
        {
            return playerCardList[id];
        }

        public List<CardDto> GetRemainCardList()
        {
            return remainCardList;
        }
        #endregion

        #region 出牌
        public bool IsWasteCard(int cardID)
        {
            return cardCollection.FindIndex((card) => { return card.Id == cardID; }) != -1;
        }

        public bool HasCard(int accountID, CardDto card)
        {
            if (playerArr.Contains(accountID))
            {
                return playerCardList[accountID].FindIndex((pc) => { return card.Id == pc.Id; }) != -1;
            }
            return false;
        }

        public bool CanPutCard(int accountID, CardDto card)
        {
            return !IsWasteCard(card.Id) && HasCard(accountID, card);
        }

        public bool CanPutCards(int accountID, List<CardDto> cards)
        {
            foreach (CardDto card in cards)
            {
                if (!CanPutCard(accountID, card))
                {
                    return false;
                }
            }
            return true;
        }


        public void CollectionCard(int accountID, CardDto card)
        {
            List<CardDto> cards = playerCardList[accountID];
            cards.RemoveAt(cards.FindIndex((c) => { return c.Id == card.Id; }));
            cardCollection.Add(card);
        }

        public void CollectionCards(int accountID, List<CardDto> cards)
        {
            List<CardDto> pcl = playerCardList[accountID];
            foreach (CardDto card in cards)
            {
                pcl.RemoveAt(pcl.FindIndex((c) => { return c.Id == card.Id; }));
                cardCollection.Add(card);
            }

        }

        public bool PutCard(int accountID, PutCardDto putCards)
        {
            if (CanPutCards(accountID, putCards.Cards) 
                && (rand.LastPutId == -1 || CardRule.CardsCompare(rand.LastCards, putCards.Cards) || rand.LastPutId == curAction))
            {
                if (putCards.CardType == CardType.BOOM || putCards.CardType == CardType.JOKE_BOOM)
                {
                    multipler *= 2;
                }

                CollectionCards(accountID, putCards.Cards);
                rand.LastPutId = accountID;
                rand.LastCards = putCards.Cards;
                rand.LastCardType = putCards.CardType;
                nextAction();
                return true;
            }
            return false;
        }

        private void nextAction()
        {
            rand.count++;
            CacheCenter.Instance.FightCache.NextActive(roomID);
            curAction = CacheCenter.Instance.FightCache.GetActiveMemberID(roomID);
        }

        public bool Grab(int accountID)
        {
            if (accountID == curAction && !IsFighting())
            {
                dizhuId = curAction;
                nextAction();
                return true;
            }
            return false;
        }

        public bool Pass(int accountID)
        {
            if (accountID == curAction)
            {
                nextAction();
                return true;
            }
            return false;
        }
        #endregion
    }

    public class Rand
    {
        public int LastPutId = -1;
        public List<CardDto> LastCards;
        public int LastCardType;
        public int count = 0;
        public int randType = 0; // 0 枪地主 ， 1 打牌
        public Rand()
        {

        }
    }
}
