using System;
using GreenEnergyHub.Ingestion.Synchronous.Application;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestTypes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HubRequestAttribute : Attribute
    {
        public HubRequestAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
