using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
    public class UserCreatedFailedException : Exception
    {
        public UserCreatedFailedException() : base("Kullanıcı Oluşturulurken Bir Hatayla Karşılaşıldı")
        {
            
        }
        public UserCreatedFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
