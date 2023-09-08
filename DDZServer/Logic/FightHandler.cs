using AhpilyServer;
using DDZServer.Cache;
using DDZServer.Model;
using Protocol.Code;
using Protocol.DTO;
using System;
using System.Collections.Generic;

namespace DDZServer.Logic
{
    public class FightHandler : IHandler
    {
        public void OnDisconnect(ClientPeer peer)
        {

        }

        public void OnReceive(ClientPeer peer, SocketMsg msg)
        {
            switch (msg.SubCode)
            {
                case FightCode.READY:
                    handleReady(peer);
                    break;
                case FightCode.SAY:
                    handleSay(peer, msg.Value as SayDto);
                    break;
                case FightCode.GRAB_DIZHU:
                    handleGrabDizhu(peer);
                    break;
                case FightCode.GIVE_UP_DIZHU:
                    handleGiveUpDizhu(peer);
                    break;
                case FightCode.PUT_CARD:
                    handlePutCard(peer, msg.Value as PutCardDto);
                    break;
                case FightCode.PASS_CARD:
                    handlePassCard(peer);
                    break;
                case FightCode.RE_DISPATCH:
                    handleRedispatch(peer);
                    break;
            }
        }

        private void handleRedispatch(ClientPeer peer)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByAccID(peer.accountID);
            if (model != null)
            {
                Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);
                model.ReDispatchCard();
                broadcastCards(room);
                broadcastSetAction(room, model.curAction, 0);
            }
        }

        private void handleReady(ClientPeer peer)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByAccID(peer.accountID);
            if (model != null)
            {
                model.AddReady(peer.accountID);
                if (model.IsAllReady())
                {
                    Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);
                    model.DispatchCard();
                    broadcastCards(room);
                    broadcastSetAction(room, model.curAction, 0);

                }
            }
        }

        private void handleSay(ClientPeer peer, SayDto sayDto)
        {
            if (sayDto != null)
            {
                Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);
                ResponseDto res = new ResponseDto(ResponseCode.SAY, sayDto);
                broadcast(peer, room, ResponseCode.SAY, res);
            }
        }

        private void handlePassCard(ClientPeer peer)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByAccID(peer.accountID);
            if (model != null && model.Pass(peer.accountID))
            {
                Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);
                if (model.curAction == model.rand.LastPutId)
                {
                    broadcastSetAction(room, model.curAction, 2);
                }
                else
                {
                    broadcastSetAction(room, model.curAction, 1);
                }
            }
        }

        private void handlePutCard(ClientPeer peer, PutCardDto putCardDto)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByAccID(peer.accountID);
            int preMultipler = model.multipler;

            if (model != null && model.PutCard(peer.accountID, putCardDto))
            {
                Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);
                broadcastSetAction(room, model.curAction, 1);
                broadcastPutCard(room, model);
                broadcastCards(room);
                if (preMultipler != model.multipler)
                {
                    broadcastRound(room, model.multipler);
                }
                if (model.GetCardList(peer.accountID).Count == 0)
                {
                    broadcastGameOver(model);
                }
            }
        }

        private void handleGiveUpDizhu(ClientPeer peer)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByAccID(peer.accountID);
            if (model != null && model.Pass(peer.accountID))
            {
                Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);
                if (model.GetRandCount() >= 3)
                {
                    model.DispatchCard();
                    broadcastCards(room);
                }

                broadcastSetAction(room, model.curAction, 0);
            }
        }

        private void handleGrabDizhu(ClientPeer peer)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByAccID(peer.accountID);
            if (model != null && model.Grab(peer.accountID))
            {
                Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(peer.accountID);

                model.AddRemainCard(peer.accountID);

                broadcastSetIdentity(room, peer.accountID);
                broadcastSetAction(room, peer.accountID, 2);
                broadcastRemainCards(room, model);
                broadcastCards(room);
            }
        }

        public void broadcastGameStart(Room room)
        {
            FightModel model = CacheCenter.Instance.FightCache.GetModelByRoomID(room.ID);
            broadcastCards(room);
            broadcastSetAction(room, model.curAction, 0);
        }

        private void broadcast(ClientPeer peer, Room room, int subCode, ResponseDto res)
        {
            int[] playerIdArr = room.GetAllPlayerIdArr();
            for (int i = 0; i < playerIdArr.Length; i++)
            {
                if (playerIdArr[i] != peer.accountID)
                {
                    ClientPeer roomPeer = CacheCenter.Instance.PlayerCache.GetClient(playerIdArr[i]);
                    roomPeer.Send(new SocketMsg(OpCode.FIGHT, subCode, res));
                }
            }
        }

        private void broadcast(Room room, int subCode, ResponseDto res)
        {
            int[] playerIdArr = room.GetAllPlayerIdArr();
            for (int i = 0; i < playerIdArr.Length; i++)
            {
                ClientPeer roomPeer = CacheCenter.Instance.PlayerCache.GetClient(playerIdArr[i]);
                roomPeer.Send(new SocketMsg(OpCode.FIGHT, subCode, res));
            }
        }

        private void broadcastSetAction(Room room, int accountID, int type)
        {
            ResponseDto actionRes = new ResponseDto(ResponseCode.SET_ACTION, new SetActionDto(accountID, type));
            broadcast(room, ResponseCode.SET_ACTION, actionRes);
        }

        private void broadcastSetIdentity(Room room, int accountID)
        {
            ResponseDto identityRes = new ResponseDto(ResponseCode.SET_IDENTITY, new SetIdentityDto(accountID, 1));
            broadcast(room, ResponseCode.SET_IDENTITY, identityRes);
        }

        private void broadcastRemainCards(Room room, FightModel model)
        {
            ResponseDto res = new ResponseDto(ResponseCode.UPDATE_REMAIN_CARDS, new UpdateRemainCardsDto(model.GetRemainCardList()));
            broadcast(room, ResponseCode.UPDATE_REMAIN_CARDS, res);
        }

        private void broadcastPutCard(Room room, FightModel model)
        {
            ResponseDto res = new ResponseDto(ResponseCode.PUT_CARD, new PutCardDto(model.rand.LastPutId, model.rand.LastCardType, model.rand.LastCards));
            broadcast(room, ResponseCode.PUT_CARD, res);
        }

        private void broadcastCards(Room room)
        {
            int[] playerIdArr = room.GetAllPlayerIdArr();
            UpdateCardsDto updateCardsDto = new UpdateCardsDto();
            ResponseDto res = new ResponseDto(ResponseCode.UPDATE_CARDS, updateCardsDto);
            SocketMsg msg = new SocketMsg(OpCode.FIGHT, ResponseCode.UPDATE_CARDS, res);

            FightModel model = CacheCenter.Instance.FightCache.GetModelByRoomID(room.ID);
            List<CardDto> cards0 = model.GetCardList(playerIdArr[0]);
            List<CardDto> cards1 = model.GetCardList(playerIdArr[1]);
            List<CardDto> cards2 = model.GetCardList(playerIdArr[2]);

            updateCardsDto.Multipler = model.multipler;
            updateCardsDto.SelfCardList = model.GetCardList(playerIdArr[0]);
            updateCardsDto.LeftCardRemainCount = cards2.Count;
            updateCardsDto.RightCardRemainCount = cards1.Count;
            CacheCenter.Instance.PlayerCache.GetClient(playerIdArr[0]).Send(msg);

            updateCardsDto.SelfCardList = model.GetCardList(playerIdArr[1]);
            updateCardsDto.LeftCardRemainCount = cards2.Count;
            updateCardsDto.RightCardRemainCount = cards0.Count;
            CacheCenter.Instance.PlayerCache.GetClient(playerIdArr[1]).Send(msg);

            updateCardsDto.SelfCardList = model.GetCardList(playerIdArr[2]);
            updateCardsDto.LeftCardRemainCount = cards0.Count;
            updateCardsDto.RightCardRemainCount = cards1.Count;
            CacheCenter.Instance.PlayerCache.GetClient(playerIdArr[2]).Send(msg);
        }

        private void broadcastRound(Room room, int multipler)
        {
            ResponseDto responseDto = new ResponseDto(ResponseCode.UPDATE_MULTIPLER, new RoundDto(multipler));
            broadcast(room, ResponseCode.UPDATE_MULTIPLER, responseDto);
        }

        private void broadcastGameOver(FightModel model)
        {
            int scoreNum = 100 * model.multipler; ;
            int[] playerArr = model.GetPlayerArr();
            int dizhuId = model.dizhuId;
            int dizhuWinMultiple = model.IsDizhuWin() ? 1 : -1;
            Room room = CacheCenter.Instance.RoomCache.GetRoomByPlayer(dizhuId);
            GameOverDto gameOverDto = new GameOverDto(playerArr, new int[3]);

            ResponseDto responseDto = new ResponseDto(ResponseCode.GAME_OVER, gameOverDto);

            for (int i = 0; i < playerArr.Length; i++)
            {
                int id = playerArr[i];

                if (id == dizhuId)
                {
                    gameOverDto.ScoreArr[i] = scoreNum * 2 * dizhuWinMultiple;
                }
                else
                {
                    gameOverDto.ScoreArr[i] = -scoreNum * dizhuWinMultiple;
                }
                PlayerModel playerModel = CacheCenter.Instance.PlayerCache.GetModel(id);
                playerModel.BeanNum += gameOverDto.ScoreArr[i];
                CacheCenter.Instance.RoomCache.PlayerCancelReady(id);
            }

            ResponseDto updateResDto = MatchHandler.getRoomRefreshRoomMemberRes(room);
            updateResDto.code = ResponseCode.MATCH_UPDATE_MEMBER;

            broadcast(room, ResponseCode.GAME_OVER, responseDto);
            broadcast(room, ResponseCode.MATCH_UPDATE_MEMBER, updateResDto);
        }
    }
}

