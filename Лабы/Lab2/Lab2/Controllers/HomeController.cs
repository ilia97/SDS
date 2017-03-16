using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Lab2.Models;
using Microsoft.AspNet.Identity;
using System.Xml;

namespace Lab2.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext context = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendMessage(string encryptedText, string text)
        {
            var userId = this.User.Identity.GetUserId();

            var user = context.Users.First(x => x.Id == userId);
            this.Log($"Encrypted text from userId \"{this.User.Identity.GetUserId()}\": \"{encryptedText}\"");

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(user.RSAXmlKeys);

            var encryptedData = GetString(rsa.Encrypt(GetBytes(text), false));
            var decryptedData = GetString(rsa.Decrypt(GetBytes(encryptedData), false));

            this.Log($"Decrypted text from userId \"{this.User.Identity.GetUserId()}\": \"{decryptedData}\"");

            return new JsonResult()
            {
                Data = decryptedData
            };
        }

        public JsonResult GetPublicKey()
        {
            return new JsonResult()
            {
                 Data = this.GetKey(),
                 JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private string GetKey()
        {
            RSA rsa = new RSACryptoServiceProvider(1024);
            var userId = this.User.Identity.GetUserId();
            context.Users.First(x => x.Id == userId).RSAXmlKeys = rsa.ToXmlString(true);
            context.SaveChanges();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rsa.ToXmlString(false));

            var publicKey = xmlDoc.InnerText;
            this.Log($"Public key for userId \"{this.User.Identity.GetUserId()}\": \"{publicKey}\"");

            return publicKey;
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private void Log(string message)
        {
            System.IO.File.AppendAllText("D:\\Projects\\SDS\\Лабы\\Lab2\\Lab2\\logs.txt", $"{message}\r\n\r\n------------------------------------------------------------\r\n\r\n");
        }
    }
}