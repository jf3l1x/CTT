using System;
using System.IO;

namespace CTT.Models
{
    public class User
    {
       
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public bool IsAdmin { get; set; }
       
        
    }
}