using System;
using System.Linq;

namespace OwnID.Configuration.Validators
{
    public class UriValidationHelper : IUriValidationHelper
    {
        public bool IsValid(string name, Uri value, bool isDevEnvironment, out string error)
        {
            error = null;

            if (value == default)
            {
                error = $"{name} is required";
                return false;
            }

            if (!value.IsWellFormedOriginalString())
            {
                error = $"{name} is not valid url";
                return false;
            }

            if (!isDevEnvironment && value.Scheme != "https")
            {
                error = $"{name}: https is required for production use";
                return false;
            }

            if (isDevEnvironment && value.Scheme != "https" && value.Scheme != "http")
            {
                error = $"{name}: https or http are supported only";
                return false;
            }

            if ((bool) value.Query?.Any())
            {
                error = $"{name} should not contain query params";
                return false;
            }

            return true;
        }
    }

    public interface IUriValidationHelper
    {
        bool IsValid(string name, Uri value, bool isDevEnvironment, out string error);
    }
}