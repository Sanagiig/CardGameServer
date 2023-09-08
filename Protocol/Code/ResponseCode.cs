using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class ResponseCode
    {
        // 通用码
        public const int SUCCESS = 0;
        public const int FAILURE = 1;
        public const int UNKNOWN = 2;
        public const int ERROR = 3;

        // 账号相关
        public const int REG_ACCOUNT_EXIST = 4;
        public const int REG_ACCOUNT_ILLEGAL = 5;
        public const int LOGIN_ACCOUNT_NOT_MATCH = 6;
        public const int LOGIN_IS_ONLINE = 7;
        public const int REG_SUCCESS = 8;
        public const int LOGIN_SUCCESS = 9;

        // 认证相关
        public const int NOT_LOGIN = 10;

        // player 相关
        public const int RES_PLAYER = 11;
        public const int PLAYER_NOT_EXIST = 12;
        public const int PLAYER_CREATE = 13;
        public const int PLAYER_UPDATE = 14;
        public const int PLAYER_REMOVE = 15;
        public const int PLAYER_GET_INFO = 16;
        public const int PLAYER_ADD_ONLINE = 17;
        public const int PLAYER_REMOVE_ONLINE = 18;

        // MATCH 相关
        public const int MATCH_ENTER = 19;
        public const int MATCH_LEAVE = 20;
        public const int MATCH_START = 21;
        public const int MATCH_IS_IN_ROOM = 22;
        public const int MATCH_READY = 23;
        public const int MATCH_CANCEL_READY = 24;
        public const int MATCH_UPDATE_MEMBER = 25;

        // FIGHT 相关
        public const int PUT_CARD = 26;
        public const int GET_CARD = 27;
        public const int PASS = 28;
        public const int SAY = 29;
        public const int GRAB_DIZHU = 30;
        public const int GIVE_UP_DIZHU = 31;
        public const int GAME_START = 32;
        public const int SET_ACTION = 33;
        public const int SET_IDENTITY = 34;
        public const int GAME_OVER = 35;
        public const int UPDATE_CARDS = 36;
        public const int UPDATE_REMAIN_CARDS = 37;
        public const int UPDATE_MULTIPLER = 38;

    }
}
