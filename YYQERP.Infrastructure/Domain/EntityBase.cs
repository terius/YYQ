using Newtonsoft.Json;
using YYQERP.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YYQERP.Infrastructure.Domain
{
    [Serializable]
    public abstract class EntityBase<TId> : IEntity
    {
        private List<BusinessRule> _brokenRules = new List<BusinessRule>();

        public TId Id { get; set; }

        [NotMapped]
        [JsonIgnore]
        public DBAction dbaction { get; set; }







        protected virtual void Validate()
        {

        }

        public IEnumerable<BusinessRule> GetBrokenRules()
        {
            _brokenRules.Clear();
            Validate();
            return _brokenRules;
        }

        protected void AddBrokenRule(BusinessRule businessRule)
        {
            _brokenRules.Add(businessRule);
        }


        public void ThrowExceptionIfInvalid(DBAction _action)
        {
            dbaction = _action;
            _brokenRules.Clear();
            Validate();
            if (_brokenRules.Count > 0)
            {
                StringBuilder issues = new StringBuilder();
                foreach (BusinessRule businessRule in _brokenRules)
                    issues.AppendLine(businessRule.Rule);

                throw new InsufficientFundsException(issues.ToString());
            }
        }

        public override bool Equals(object entity)
        {
            return entity != null
               && entity is EntityBase<TId>
               && this == (EntityBase<TId>)entity;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(EntityBase<TId> entity1,
           EntityBase<TId> entity2)
        {
            if ((object)entity1 == null && (object)entity2 == null)
            {
                return true;
            }

            if ((object)entity1 == null || (object)entity2 == null)
            {
                return false;
            }

            if (entity1.Id.ToString() == entity2.Id.ToString())
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(EntityBase<TId> entity1,
            EntityBase<TId> entity2)
        {
            return (!(entity1 == entity2));
        }
    }

}
