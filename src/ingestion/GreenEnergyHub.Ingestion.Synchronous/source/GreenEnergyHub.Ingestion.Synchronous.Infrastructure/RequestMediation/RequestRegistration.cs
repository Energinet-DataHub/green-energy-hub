using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestTypes;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestMediation
{
    public class RequestRegistration
    {
        internal RequestRegistration([NotNull] Type requestType)
        {
            RequestName = GetRequestName(requestType);
            RequestType = requestType;
        }

        internal RequestRegistration(string name, Type requestType)
        {
            RequestName = name;
            RequestType = requestType;
        }

        internal string RequestName { get; }

        internal Type RequestType { get; }

        private static string GetRequestName(MemberInfo memberInfo)
        {
            var attr = memberInfo.GetCustomAttribute<HubRequestAttribute>();
            return attr?.Name ?? memberInfo.Name;
        }
    }
}
