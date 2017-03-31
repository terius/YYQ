using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace YYQERP.Services
{

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class NotEqualsAttribute : ValidationAttribute, IClientValidatable
    {
        object _v;
        public NotEqualsAttribute(object v)
        {
            _v = v;
        }

        public override bool IsValid(object value)
        {
            if (value != null && _v.ToString().Equals(value.ToString()))
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return name + "绝对不能够等于 " + _v;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "notequals",
                ErrorMessage = String.IsNullOrEmpty(ErrorMessage) ? FormatErrorMessage(string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName) : ErrorMessage
            };
            validationRule.ValidationParameters.Add("nval", this._v);

            yield return validationRule;
        }


    }
}
