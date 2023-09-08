using AhpilyServer;
using AhpilyServer.Utils.MonoHelps;
using DDZServer.Cache;
using Protocol.Code;
using Protocol.DTO;
using System;

namespace DDZServer.Logic
{
    public class AccountHandler : IHandler
    {
        public void OnReceive(ClientPeer peer, SocketMsg msg)
        {
            switch (msg.SubCode)
            {
                case AccountCode.REGISTER_REQ:
                    AccountDto registerDto = msg.Value as AccountDto;
                    Console.WriteLine("register {0} ,{1}", registerDto.account, registerDto.psw);
                    register(peer, registerDto);
                    break;
                case AccountCode.LOGIN:
                    AccountDto loginDto = msg.Value as AccountDto;
                    Console.WriteLine("login {0} ,{1}", loginDto.account, loginDto.psw);
                    login(peer, loginDto);
                    break;
                default:
                    Console.WriteLine("unknown code {0},{1}", msg.OpCode, msg.SubCode);
                    break;
            }
        }

        public void OnDisconnect(ClientPeer peer)
        {

            Console.WriteLine("[{0}] disconnect", peer.account);
            CacheCenter.Instance.AccountCache.RemoveClient(peer.account);
        }

        private void register(ClientPeer peer, AccountDto data)
        {
            int resCode = ResponseCode.REG_SUCCESS;
            if (data.account.Length < 4 || data.account.Length > 16 || data.psw.Length < 4 || data.psw.Length > 16)
            {
                resCode = ResponseCode.REG_ACCOUNT_ILLEGAL;
            }
            else if (CacheCenter.Instance.AccountCache.IsExist(data.account))
            {
                resCode = ResponseCode.REG_ACCOUNT_EXIST;
            }

            if (resCode == ResponseCode.REG_SUCCESS)
            {
                CacheCenter.Instance.AccountCache.AddModel(data.account, data.psw);
            }
            peer.Send(OpCode.ACCOUNT, AccountCode.REGISTER_REQ, (object)resCode);
        }

        private void login(ClientPeer peer, AccountDto data)
        {
            int resCode = ResponseCode.LOGIN_SUCCESS;
            if (!CacheCenter.Instance.AccountCache.IsMatch(data.account, data.psw))
            {
                resCode = ResponseCode.LOGIN_ACCOUNT_NOT_MATCH;
            }
            else if (CacheCenter.Instance.AccountCache.IsOnline(data.account))
            {
                resCode = ResponseCode.LOGIN_IS_ONLINE;
            }

            if (resCode == ResponseCode.LOGIN_SUCCESS)
            {
                CacheCenter.Instance.AccountCache.AddClient(data.account, peer);
                peer.account = data.account;
                peer.accountID = CacheCenter.Instance.AccountCache.GetModel(data.account).ID;

                if (CacheCenter.Instance.PlayerCache.IsExist(peer.accountID))
                {
                    CacheCenter.Instance.PlayerCache.AddOnline(peer);
                }
            }
            peer.Send(OpCode.ACCOUNT, AccountCode.LOGIN, (object)resCode);
        }
    }
}
