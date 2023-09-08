using DDZServer.Model;
using Protocol.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Cache
{
    public class Room
    {
        private static int id = 0;
        public int ID;
        public SynchronizedCollection<int> playerList = new SynchronizedCollection<int>();
        public SynchronizedCollection<int> readyPlayerList = new SynchronizedCollection<int>();
        public SynchronizedCollection<int> leavePlayerList = new SynchronizedCollection<int>();

        public Room()
        {
            id++;
            ID = id;
        }
        public void AddPlayer(int playerID)
        {
            playerList.Add(playerID);
        }

        public void RemovePlayer(int id)
        {
            PlayerCancelReady(id);
            playerList.Remove(id);
        }

        public void PlayerLeave(int id)
        {
            if (IsReady(id))
            {
                leavePlayerList.Add(id);
            }
        }

        public void ClearRoom()
        {
            playerList.Clear();
            readyPlayerList.Clear();
            leavePlayerList.Clear();
        }

        public void PlayerReady(int playerID)
        {
            readyPlayerList.Add(playerID);
        }

        public void PlayerCancelReady(int playerID)
        {
            readyPlayerList.Remove(playerID);
        }

        public bool IsReady(int id)
        {
            return readyPlayerList.Contains(id);
        }
        public bool IsAllReady()
        {
            return readyPlayerList.Count == 3;
        }

        public bool IsEmpty()
        {
            return playerList.Count == 0;
        }

        public bool CanEnter()
        {
            return playerList.Count < 3;
        }

        public bool IsInRoom(int id)
        {
            return playerList.Contains(id);
        }

        public int[] GetReadyIdArr()
        {
            return readyPlayerList.ToArray();
        }

        public int[] GetAllPlayerIdArr()
        {
            return playerList.ToArray();
        }

        public PlayerModel[] GetAllPlayerInfo()
        {
            PlayerModel[] modelList = new PlayerModel[playerList.Count];

            for (var i = 0; i < modelList.Length; i++)
            {
                PlayerModel model = CacheCenter.Instance.PlayerCache.GetModel(playerList[i]);
                modelList[i] = model;
            }

            return modelList;
        }

    }
    public class RoomCache
    {
        public SynchronizedCollection<Room> avaliableRoomList = new SynchronizedCollection<Room>();
        public SynchronizedCollection<Room> busyRoomList = new SynchronizedCollection<Room>();
        public SynchronizedCollection<Room> fightingRoomList = new SynchronizedCollection<Room>();
        public ConcurrentDictionary<int, Room> player2RoomDic = new ConcurrentDictionary<int, Room>();
        public ConcurrentDictionary<int, Room> id2RoomDic = new ConcurrentDictionary<int, Room>();
        public RoomCache() { }

        public bool IsInRoom(int id)
        {
            return player2RoomDic.ContainsKey(id);
        }

        public Room GetRoomByPlayer(int id)
        {
            if (IsInRoom(id))
            {
                return player2RoomDic[id];
            }
            return null;
        }

        public Room GetAvaliableRoom()
        {
            Room room;
            if (avaliableRoomList.Count > 0)
            {
                return avaliableRoomList[0];
            }
            else
            {
                room = new Room();
                id2RoomDic.TryAdd(room.ID, room);
            }

            avaliableRoomList.Add(room);
            return room;
        }

        public void PlayerEnter(int id)
        {
            if (IsInRoom(id))
            {
                Console.WriteLine("ID:{0} - {1} 重复进入房间", CacheCenter.Instance.PlayerCache.GetModel(id).Name);
                return;
            }

            Room room = GetAvaliableRoom();
            room.AddPlayer(id);
            player2RoomDic.TryAdd(id, room);
            if (!room.CanEnter())
            {
                avaliableRoomList.Remove(room);
                busyRoomList.Add(room);
            }
        }

        public void PlayerReady(int id)
        {
            if (IsInRoom(id))
            {
                player2RoomDic[id].PlayerReady(id);
            }
        }

        public void PlayerCancelReady(int id)
        {
            if (IsInRoom(id))
            {
                player2RoomDic[id].PlayerCancelReady(id);
            }
        }

        public void PlayerLeave(int id)
        {
            if (IsInRoom(id))
            {
                Room room = GetRoomByPlayer(id);
                room.RemovePlayer(id);
                player2RoomDic.TryRemove(id, out room);

                if (busyRoomList.Contains(room))
                {
                    avaliableRoomList.Remove(room);
                }
                else
                {
                    if (avaliableRoomList.Remove(room))
                    {
                        avaliableRoomList.Add(room);
                    }
                }
            }
        }

        public Room GetRoomByID(int id)
        {
            return id2RoomDic[id];
        }
    }
}
