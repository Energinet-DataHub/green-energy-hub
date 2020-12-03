// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GreenEnergyHub.Messaging.MessageTypes;

namespace GreenEnergyHub.Messaging.MessageRouting
{
    public class MessageRegistration
    {
        public MessageRegistration([NotNull] Type messageType)
        {
            MessageName = GetMessageName(messageType);
            MessageType = messageType;
        }

        public MessageRegistration(string name, Type messageType)
        {
            MessageName = name;
            MessageType = messageType;
        }

        internal string MessageName { get; }

        internal Type MessageType { get; }

        private static string GetMessageName(MemberInfo memberInfo)
        {
            var attr = memberInfo.GetCustomAttribute<HubMessageAttribute>();
            return attr?.Name ?? memberInfo.Name;
        }
    }
}
