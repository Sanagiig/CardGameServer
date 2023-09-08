using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    [Serializable]
    public class ResMatchDto
    {
        public PlayerInfoDto[] PlayerArr;
        public int[] readyArr;

        public ResMatchDto(PlayerInfoDto[] infoArr, int[] readyArr)
        {
            this.PlayerArr = infoArr;
            this.readyArr = readyArr;
        }
    }
}
