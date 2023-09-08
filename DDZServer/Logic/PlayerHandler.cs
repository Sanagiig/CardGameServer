using AhpilyServer;
using DDZServer.Cache;
using DDZServer.Model;
using Protocol.Code;
using Protocol.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Logic
{
    internal class PlayerHandler : IHandler
    {
        public void OnDisconnect(ClientPeer peer)
        {
            removeOnline(peer);
        }

        public void OnReceive(ClientPeer peer, SocketMsg msg)
        {
            switch (msg.SubCode)
            {
                case Player.CREATE_PLAYER:
                    createPlayer(peer, msg);
                    break;
                case Player.GET_PLAYER:
                    getPlayer(peer, msg);
                    break;
                case Player.REMOVE_PLAYER:
                    removePlayer(peer, msg);
                    break;
                case Player.UPDATE_PLAYER:
                    updatePlayer(peer, msg);
                    break;
                case Player.ADD_ONLINE:
                    addOnline(peer, msg);
                    break;
            }
        }

        

        private void createPlayer(ClientPeer peer, SocketMsg msg)
        {
            ResponseDto res = new ResponseDto();
            res.code = ResponseCode.SUCCESS;

            if (peer.account == null)
            {
                res.code = ResponseCode.NOT_LOGIN;
            }
            else
            {
                PlayerCreateDto dto = msg.Value as PlayerCreateDto;
                PlayerModel model = CacheCenter.Instance.PlayerCache.CreatePlayer(dto.name, peer.accountID);
                PlayerInfoDto info = new PlayerInfoDto(model.Id, model.AccountID, model.Name, model.BeanNum, model.WinCount, model.LoseCount, model.RunCount, model.Level, model.Exp);

                CacheCenter.Instance.PlayerCache.AddOnline(peer);
                res.data = info;
            }

            peer.Send(OpCode.PLAYER, ResponseCode.PLAYER_CREATE, res);
        }

        private void getPlayer(ClientPeer peer, SocketMsg msg)
        {
            ResponseDto res = new ResponseDto();
            res.code = ResponseCode.SUCCESS;

            if (!CacheCenter.Instance.PlayerCache.IsExist(peer.accountID))
            {
                res.code = ResponseCode.PLAYER_NOT_EXIST;
            }
            else if (!CacheCenter.Instance.PlayerCache.IsOnline(peer.accountID))
            {
                res.code = ResponseCode.NOT_LOGIN;
            }
            else
            {
                PlayerModel model = CacheCenter.Instance.PlayerCache.GetModel(peer.accountID);
                PlayerInfoDto dto = new PlayerInfoDto(model.Id, model.AccountID, model.Name, model.BeanNum, model.WinCount, model.LoseCount, model.RunCount, model.Level, model.Exp);
                res.data = dto;
            }

            peer.Send(OpCode.PLAYER, ResponseCode.PLAYER_GET_INFO, res);
        }

        private void removePlayer(ClientPeer peer, SocketMsg msg)
        {
            int resCode = ResponseCode.SUCCESS;
            if (!CacheCenter.Instance.PlayerCache.IsExist(peer.accountID))
            {
                resCode = ResponseCode.PLAYER_NOT_EXIST;
            }

            if (resCode == ResponseCode.SUCCESS)
            {
                CacheCenter.Instance.PlayerCache.RemovePlayer(peer.accountID);
            }
            peer.Send(OpCode.PLAYER, ResponseCode.PLAYER_REMOVE, resCode);
        }

        private void updatePlayer(ClientPeer peer, SocketMsg msg)
        {
            peer.Send(OpCode.PLAYER, ResponseCode.PLAYER_REMOVE, ResponseCode.SUCCESS);
        }

        private void addOnline(ClientPeer peer, SocketMsg msg)
        {
            int resCode = ResponseCode.SUCCESS;
            if (peer.account == null)
            {
                resCode = ResponseCode.NOT_LOGIN;
            }
            else if (!CacheCenter.Instance.PlayerCache.IsExist(peer.accountID))
            {
                resCode = ResponseCode.PLAYER_NOT_EXIST;
            }

            if (resCode == ResponseCode.SUCCESS)
            {
                CacheCenter.Instance.PlayerCache.AddOnline(peer);
            }

            peer.Send(OpCode.PLAYER, ResponseCode.PLAYER_ADD_ONLINE, resCode);
        }

        private void removeOnline(ClientPeer peer)
        {
            CacheCenter.Instance.PlayerCache.RemoveOnline(peer.accountID);
        }
    }
}
