using YYQERP.Infrastructure.Helpers;
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
    public class IdCardAttribute : ValidationAttribute, IClientValidatable
    {


        public override bool IsValid(object value)
        {
            if (value != null && StringHelper.CheckIDCard(value.ToString()))
            {
                return true;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return "身份证格式错误";
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule validationRule = new ModelClientValidationRule()
            {
                ValidationType = "checkidcard",
                ErrorMessage = String.IsNullOrEmpty(ErrorMessage) ? FormatErrorMessage(string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName) : ErrorMessage
            };
            // validationRule.ValidationParameters.Add("nval", this._v);

            yield return validationRule;
        }


    }
}
