using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    [Serializable]
    public class ResponseDto
    {
        public int code;
        public object data;
        public ResponseDto() { }
        public ResponseDto(int code, object data)
        {
            this.code = code;
            this.data = data;
        }
        public ResponseDto(int code)
        {
            this.code = code;
        }
    }
}
