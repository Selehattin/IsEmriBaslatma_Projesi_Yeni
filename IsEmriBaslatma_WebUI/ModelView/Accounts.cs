using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsEmriBaslatma_WebUI.ModelView
{
    public class Accounts
    {
        public int id { get; set; }
        public Nullable<int> rollID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Nullable<bool> Activate { get; set; }
    }
}