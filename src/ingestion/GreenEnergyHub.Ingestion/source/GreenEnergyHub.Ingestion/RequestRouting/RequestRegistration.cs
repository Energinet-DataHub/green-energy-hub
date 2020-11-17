using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GreenEnergyHub.Ingestion.RequestTypes;

namespace GreenEnergyHub.Ingestion.RequestRouting
{
    public class RequestRegistration
    {
        public RequestRegistration([NotNull] Type requestType)
        {
            RequestName = GetRequestName(requestType);
            RequestType = requestType;
        }

        public RequestRegistration(string name, Type requestType)
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
