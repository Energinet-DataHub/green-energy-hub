using System;

namespace GreenEnergyHub.Ingestion.RequestTypes
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
