using System;

namespace YYQERP.Infrastructure.Domain
{
    [Serializable]
    public class ValueObjectIsInvalidException : Exception
    {
        public ValueObjectIsInvalidException(string message)
            : base(message)
        {

        }
    }
}