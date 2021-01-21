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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Energinet.DataHub.MarketData.Domain.SeedWork
{
    public abstract class EnumerationType : IComparable
    {
        protected EnumerationType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; private set; }

        public int Id { get; private set; }

        public static bool operator ==(EnumerationType left, EnumerationType right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(EnumerationType left, EnumerationType right)
        {
            return !(left == right);
        }

        public static bool operator <(EnumerationType left, EnumerationType right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(EnumerationType left, EnumerationType right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(EnumerationType left, EnumerationType right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(EnumerationType left, EnumerationType right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }

        public static IEnumerable<T> GetAll<T>()
                   where T : EnumerationType
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public static int AbsoluteDifference(EnumerationType firstValue, EnumerationType secondValue)
        {
            if (firstValue is null)
            {
                throw new ArgumentNullException(nameof(firstValue));
            }

            if (secondValue is null)
            {
                throw new ArgumentNullException(nameof(secondValue));
            }

            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        public static T FromValue<T>(int value)
            where T : EnumerationType
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromName<T>(string name)
            where T : EnumerationType
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return matchingItem;
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (!(obj is EnumerationType otherValue))
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public int CompareTo(object? other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return Id.CompareTo(((EnumerationType)other).Id);
        }

        private static T Parse<T, TValue>(TValue value, string description, Func<T, bool> predicate)
            where T : EnumerationType
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            return matchingItem ?? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
        }
    }
}
