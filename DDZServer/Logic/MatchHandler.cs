using AhpilyServer;
using DDZServer.Cache;
using DDZServer.Model;
using DDZServer.Utils;
using Protocol.Code;
using Protocol.DTO;

namespace DDZServer.Logic
{
    public class MatchHandler : IHandler
    {
        public void OnDisconnect(ClientPeer peer)
        {
            CacheCenter.Instance.RoomCache.PlayerLeave(peer.accountID);
        }

        public void OnReceive(ClientPeer peer, SocketMsg msg)
        {
            switch (msg.SubCode)
            {
                case MatchCode.ENTER:
                    handleEnter(peer);
                    break;
                case MatchCode.LEAVE:
                    handleLeave(peer);
                    break;
                case MatchCode.READY:
                    handleReady(peer);
                    break;
                case MatchCode.CANCEL_READY:
                    handleCancelReady(peer);
                    break;
            }
        }

        public static ResponseDto genMatchResponse(ClientPeer peer)
        {
            ResponseDto response = new ResponseDto();
            response.code = ResponseCode.SUCCESS;

            if (!CacheCenter.Instance.PlayerCache.IsOnline(peer.accountID))
            {
                response.code = ResponseCode.NOT_LOGIN;
            }

            return response;
        }

        private void handleEnter(ClientPeer peer)
        {
            ResponseDto res = genMatchResponse(peer);
            RoomCache rc = CacheCenter.Instance.RoomCache;

            if (rc.IsInRoom(peer.accountID))
            {
                return;
            }

            if (res.code == ResponseCode.SUCCESS)
            {
                Room room;
                rc.PlayerEnter(peer.accountID);
                room = rc.GetRoomByPlayer(peer.accountID);
                BroadcastRoomMember(room, ResponseCode.MATCH_UPDATE_MEMBER);
            }

            peer.Send(OpCode.MATCH, ResponseCode.MATCH_ENTER, res);
        }

        private void handleLeave(ClientPeer peer)
        {
            ResponseDto res = genMatchResponse(peer);
            RoomCache rc = CacheCenter.Instance.RoomCache;

            if (!rc.IsInRoom(peer.accountID))
            {
                return;
            }

            if (res.code == ResponseCode.SUCCESS)
            {
                Room room = rc.GetRoomByPlayer(peer.accountID);
                rc.PlayerLeave(peer.accountID);
                BroadcastRoomMember(room, ResponseCode.MATCH_UPDATE_MEMBER);
            }


            peer.Send(OpCode.MATCH, ResponseCode.MATCH_LEAVE, res);
        }

        private void handleReady(ClientPeer peer)
        {
            ResponseDto res = genMatchResponse(peer);
            RoomCache rc = CacheCenter.Instance.RoomCache;

            if (!rc.IsInRoom(peer.accountID))
            {
                return;
            }

            if (res.code == ResponseCode.SUCCESS)
            {
                Room room = rc.GetRoomByPlayer(peer.accountID);
                room.PlayerReady(peer.accountID);
                BroadcastRoomMember(room, ResponseCode.MATCH_UPDATE_MEMBER);

                if (room.IsAllReady())
                {
                    CacheCenter.Instance.FightCache.GameStart(room);
                    BroadcastRoomMember(room, ResponseCode.GAME_START);
                }
            }

            peer.Send(OpCode.MATCH, ResponseCode.MATCH_READY, res);
        }

        private void handleCancelReady(ClientPeer peer)
        {
            ResponseDto res = genMatchResponse(peer);
            RoomCache rc = CacheCenter.Instance.RoomCache;

            if (!rc.IsInRoom(peer.accountID))
            {
                return;
            }

            if (res.code == ResponseCode.SUCCESS)
            {
                Room room = rc.GetRoomByPlayer(peer.accountID);
                room.PlayerCancelReady(peer.accountID);
                BroadcastRoomMember(room, ResponseCode.MATCH_UPDATE_MEMBER);
            }

            peer.Send(OpCode.MATCH, ResponseCode.MATCH_CANCEL_READY, res);
        }

        public static ResponseDto getRoomRefreshRoomMemberRes(Room room)
        {
            PlayerModel[] playerArr = room.GetAllPlayerInfo();
            PlayerInfoDto[] infoArr = new PlayerInfoDto[playerArr.Length];

            int[] readyArr = room.GetReadyIdArr();
            ResponseDto res = new ResponseDto();
            ResMatchDto matchDto = new ResMatchDto(infoArr, readyArr);

            res.data = matchDto;

            for (int i = 0; i < playerArr.Length; i++)
            {
                if (playerArr[i] != null)
                {
                    infoArr[i] = DtoTrans.GetPlayerInfoDto(playerArr[i]);
                }
            }

            return res;
        }

        private void broadcast(int[] idArr, ResponseDto res)
        {
            SocketMsg msg = new SocketMsg(OpCode.MATCH, res.code, null);
            foreach (int id in idArr)
            {
                ClientPeer peer = CacheCenter.Instance.PlayerCache.GetClient(id);
                msg.Value = res;
                peer?.Send(msg);
            }
        }

        public void BroadcastRoomMember(Room room, int resCode)
        {
            switch (resCode)
            {
                case ResponseCode.MATCH_UPDATE_MEMBER:
                    ResponseDto res = getRoomRefreshRoomMemberRes(room);
                    res.code = resCode;
                    broadcast(room.GetAllPlayerIdArr(), res);
                    break;
                case ResponseCode.GAME_START:
                    broadcast(room.GetAllPlayerIdArr(),new ResponseDto(resCode));
                    break;
            }
        }
    }
}
