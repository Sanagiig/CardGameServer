using DDZServer.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Cache
{
    public class FightCache
    {
/*        #region Room Cache
        private SynchronizedCollection<int> fightingRoomList = new SynchronizedCollection<int>();
        public bool IsFighting(int roomId)
        {
            return fightingRoomList.Contains(roomId);
        }

        public void AddFight(int roomId)
        {
            if (!IsFighting(roomId))
            {
                fightingRoomList.Add(roomId);
            }
        }

        public void RemoveFight(int roomId)
        {
            if (IsFighting(roomId))
            {
                fightingRoomList.Remove(roomId);
            }
        }
        #endregion*/

        #region Room Info Cache
        private ConcurrentDictionary<int,int> roomActiveMemberDic = new ConcurrentDictionary<int, int>();
        public void SetNextActive(int roomId,int accountID)
        {
            roomActiveMemberDic[roomId] = accountID;
        }

        public int NextActive(int roomId)
        {
            int id = GetNextActiveMemberID(roomId);
            if(id != -1)
            {
                SetNextActive(roomId, id);
            }
            return id;
        }

        public int GetActiveMemberID(int roomId)
        {
            return roomActiveMemberDic[roomId];
        }

        public int GetNextActiveMemberID(int roomId)
        {
            int[] playerArr = CacheCenter.Instance.RoomCache.GetRoomByID(roomId)?.GetAllPlayerIdArr();
            int id = GetActiveMemberID(roomId);
            if(id != null && playerArr != null)
            {
                int idx = Array.IndexOf(playerArr, id);
                if(idx != -1)
                {
                    idx = (idx + 1) % playerArr.Length;
                    return playerArr[idx];
                }
            }
            return -1;
        }
        #endregion

        #region fightModel
        public ConcurrentDictionary<int,FightModel> room2FightModelDic = new ConcurrentDictionary<int,FightModel>();
        public void GameStart(Room room)
        {
            FightModel model = new FightModel(room.ID,room.GetAllPlayerIdArr());
            room2FightModelDic[room.ID] = model;
        }

        public FightModel GetModelByAccID(int accID)
        {
            int roomID = CacheCenter.Instance.RoomCache.GetRoomByPlayer(accID).ID;
            return room2FightModelDic[roomID];
        }
        
        public FightModel GetModelByRoomID(int roomID)
        {
            return room2FightModelDic[roomID];
        }
        #endregion
    }

    #region Round Manager
    public class RoundModel
    {
        public int CurAcountID;
        public int BiggestAccountID;
        public int LastLength;
        public int LastWeight;
        public int LastCardColor;

        public RoundModel()
        {
            CurAcountID = -1;
            BiggestAccountID = -1;
            LastLength = -1;
            LastWeight = -1;
            LastCardColor = -1;
        }

        public void Start(int id)
        {
            CurAcountID = id;
            BiggestAccountID = id;
        }

        public void Change(int len, int type, int weight, int accoutID)
        {
            BiggestAccountID = accoutID;
            LastLength = len;
            LastCardColor = type;
            LastWeight = weight;
        }

        public void Turn(int accoutnID)
        {
            CurAcountID = accoutnID;
        }
    }
    #endregion
}
