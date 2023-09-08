using AhpilyServer;
using AhpilyServer.Utils.ConcurrentVal;
using DDZServer.Model;
using System.Collections.Concurrent;

namespace DDZServer.Cache
{
    public class AccountCache
    {
        private ConcurrentInt curID = new ConcurrentInt(0);

        public AccountCache() {
            AddModel("1111", "1111");
            AddModel("2222", "2222");
            AddModel("3333", "3333");
        }

        #region Model 相关
        private ConcurrentDictionary<string, AccountModel> accModelDic = new ConcurrentDictionary<string, AccountModel>();
        public bool IsExist(string account)
        {
            return account != null && accModelDic.ContainsKey(account);
        }

        public bool IsMatch(string account, string password)
        {
            AccountModel model = GetModel(account);
            if (model != null)
            {
                return model.Password.Equals(password);
            }

            return false;
        }

        public void AddModel(string account, string password)
        {
            AccountModel accModel = new AccountModel(curID.Increase().Value, account, password);
            accModelDic.TryAdd(account, accModel);
        }
        public AccountModel GetModel(string account)
        {
            if (!IsExist(account))
            {
                return null;
            }
            return accModelDic[account];
        }

        public void RemoveModel(string account)
        {
            AccountModel model;
            accModelDic.TryRemove(account, out model);
        }

        #endregion

        #region 连接相关
        private ConcurrentDictionary<string, ClientPeer> accClientDic = new ConcurrentDictionary<string, ClientPeer>();
        public bool IsOnline(string account)
        {
            return account != null && accClientDic.ContainsKey(account);
        }

        public void AddClient(string account, ClientPeer client)
        {
            if (accClientDic.ContainsKey(account))
            {
                accClientDic.TryAdd(account, client);
            }
        }

        public ClientPeer GetClient(string account)
        {
            if (IsOnline(account))
            { 
                return accClientDic[account];
            }
            return null;
        }

        public void RemoveClient(string account)
        {
            if (IsOnline(account))
            {
                accClientDic.TryRemove(account, out _);
            }
        }
        #endregion
    }
}
