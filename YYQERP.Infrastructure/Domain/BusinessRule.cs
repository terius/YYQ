using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.Domain
{
    [Serializable]
    public class BusinessRule
    {
        private Property _property;
        private string _rule;

        public BusinessRule(Property property, string rule)
        {
            this._property = property;
            this._rule = rule;
        }

        public Property Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public string Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }
    }

    public enum Property
    {
        NotEmpty,
        DateTimeError,
        NumberError,
        DupError
    }

}
