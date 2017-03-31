using YYQERP.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.Domain
{
    public interface IEntity
    {
       
         void ThrowExceptionIfInvalid(DBAction action);
    }

}
