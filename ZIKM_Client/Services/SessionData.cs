using System;
using ZIKM_Client.Interfaces;

namespace ZIKM_Client.Services
{
    static class SessionData
    {
        public static Guid SessionId { get; set; }
        public static IProvider Provider { get; set; }
    }
}
