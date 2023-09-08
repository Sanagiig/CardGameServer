using AhpilyServer;
using AhpilyServer.Utils.ConcurrentVal;
using DDZServer.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Cache
{
    public class PlayerCache
    {
        #region 玩家数据
        private ConcurrentInt curID = new ConcurrentInt(0);
        private ConcurrentDictionary<int, PlayerModel> playerModelDic = new ConcurrentDictionary<int, PlayerModel>();
        private ConcurrentDictionary<int, int> account2PlayerDic = new ConcurrentDictionary<int, int>();

        public PlayerCache()
        {
            CreatePlayer("1111", 1);
            CreatePlayer("2222", 2);
            CreatePlayer("3333", 3);
        }
        public bool IsExist(int accountID)
        {
            return account2PlayerDic.ContainsKey(accountID);
        }

        public PlayerModel CreatePlayer(string name, int accountID)
        {
            PlayerModel model = new PlayerModel(curID.Increase().Value, name, accountID);
            playerModelDic.TryAdd(model.Id, model);
            account2PlayerDic.TryAdd(accountID, model.Id);
            return model;
        }

        public void RemovePlayer(int accountID)
        {
            if (IsExist(accountID))
            {
                PlayerModel playerModel;
                int playerID;
                playerModelDic.TryRemove(accountID, out playerModel);
                account2PlayerDic.TryRemove(accountID, out playerID);
            }
        }

        public PlayerModel GetModel(int accountID)
        {
            if (IsExist(accountID))
            {
                return playerModelDic[accountID];
            }
            return null;
        }

        public int GetPlayerID(int accountID)
        {
            if (IsExist(accountID))
            {
                return account2PlayerDic[accountID];
            }
            return -1;
        }

        #endregion

        #region 在线数据
        private ConcurrentDictionary<int, ClientPeer> account2ClientDic = new ConcurrentDictionary<int, ClientPeer>();

        public bool IsOnline(int accountID)
        {
            return account2ClientDic.ContainsKey(accountID);
        }
        public void AddOnline(ClientPeer client)
        {
            if (!IsOnline(client.accountID))
            {
                account2ClientDic.TryAdd(client.accountID, client);
            }
        }

        public void RemoveOnline(int accountID)
        {
            if (IsOnline(accountID))
            {
                account2ClientDic.TryRemove(accountID, out _);
            }
        }

        public ClientPeer GetClient(int accountID)
        {
            if (IsOnline(accountID))
            {
                return account2ClientDic[accountID];
            }

            return null;
        }
        #endregion 
    }
}
