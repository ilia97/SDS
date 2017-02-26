using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptWithDES(string text, string key)
        {
            //for(int i = 0; i < text.Length; i += 2)
            //{
            //    var word64 = 
            //}

            return "";
        }

        private string EncryptWithDES64Bit(string text, string key)
        {
            return "";
        }
    }
}
