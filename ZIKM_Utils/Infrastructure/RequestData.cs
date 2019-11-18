using System;

namespace ZIKM_Utils.Infrastructure
{
    public struct RequestData
    {
        public Guid SessionId { get; set; }
        public int Operation { get; set; }
        public string Property { get; set; }

        public RequestData(Guid sessionId, int operation, string property)
        {
            SessionId = sessionId;
            Operation = operation;
            Property = property;
        }
    }
}