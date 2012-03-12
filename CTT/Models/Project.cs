using System;
using System.Collections.Generic;

namespace CTT.Models
{
    public class Project
    {
        public Project()
        {
            AllowedUsers=new List<String>();
        }
        public string Id { get; set; }
        public string Client { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public IList<String> AllowedUsers { get; set; }
    }
}