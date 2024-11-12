
using System.Reflection;

namespace IdentityService.Tests.Helpers
{
    internal class ResultHelper
    {
        public static IEnumerable<object> GetValuesFromObject(object obj) 
        {
            Type objType = obj.GetType();
            PropertyInfo[] objProperties = objType.GetProperties();
            return objProperties.Select(p => p.GetValue(obj));
        }
    }
}