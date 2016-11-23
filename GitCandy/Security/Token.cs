﻿using System;
using System.Web;

namespace GitCandy.Security
{
    public class Token
    {
        private const String ContentKey = "GitCandyToken";
        private const double AuthPeriod = 21600; // 15 days
        private const double RenewalPeriod = 1440; // 1 day

        private Token() { }

        public Token(String authCode, Int32 userID, String username, String nickname, bool isSystemAdministrator, DateTime? expires = null)
        {
            AuthCode = new Guid(authCode);
            UserID = userID;
            Username = username;
            Nickname = nickname;
            IsSystemAdministrator = isSystemAdministrator;

            var now = DateTime.Now;
            IssueDate = now;
            Expires = expires ?? now.AddMinutes(AuthPeriod);
        }

        public static Token Current
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null) return null;

                return context.Items[ContentKey] as Token;
            }
            set
            {
                var context = HttpContext.Current;
                if (context == null) return;

                context.Items[ContentKey] = value;
            }
        }

        public Guid AuthCode { get; private set; }
        public Int32 UserID { get; private set; }
        public String Username { get; private set; }
        public String Nickname { get; private set; }
        public bool IsSystemAdministrator { get; private set; }

        public DateTime Expires { get; private set; }
        public DateTime IssueDate { get; private set; }
        public String LastIp { get; set; }

        public bool Expired { get { return Expires > DateTime.Now.AddMinutes(AuthPeriod); } }

        public bool RenewIfNeed()
        {
            var now = DateTime.Now;
            if (Expires > now && (Expires - now).TotalMinutes < AuthPeriod - RenewalPeriod)
            {
                Expires = now.AddMinutes(AuthPeriod);
                return true;
            }
            return false;
        }

        public static DateTime AuthorizationExpires
        {
            get
            {
                return DateTime.Now.AddMinutes(AuthPeriod);
            }
        }
    }
}