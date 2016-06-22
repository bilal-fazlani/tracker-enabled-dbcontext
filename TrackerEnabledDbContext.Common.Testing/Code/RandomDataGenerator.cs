using System;
using System.Collections.Generic;

namespace TrackerEnabledDbContext.Common.Testing.Code
{
    public class RandomDataGenerator
    {
        readonly Dictionary<Type, Func<object>> _factoryMap = new Dictionary<Type, Func<object>>();

        public RandomDataGenerator()
        {
            _factoryMap.Add(typeof(int), GetRandomNumber);
            _factoryMap.Add(typeof(bool), GetRandomBoolean);
            _factoryMap.Add(typeof(char), GetRandomChar);
            _factoryMap.Add(typeof(DateTime), GetRandomDate);
            _factoryMap.Add(typeof(string),GetRandomText);
            _factoryMap.Add(typeof(double), GetRandomNumber);

            _factoryMap.Add(typeof(int?), GetRandomNumber);
            _factoryMap.Add(typeof(DateTime?), GetRandomDate);
            _factoryMap.Add(typeof(bool?), GetRandomBoolean);
            _factoryMap.Add(typeof(char?), GetRandomChar);
            _factoryMap.Add(typeof(double?), GetRandomNumber);
        }

        public T Get<T>()
        {
            Func<object> factory = null;
            bool valueFound = _factoryMap.TryGetValue(typeof (T), out factory);
            if (valueFound)
            {
                return (T) factory();
            }

            throw new NotImplementedException($"factory for {typeof(T).Name} is not implemented");
        }

        public object Get(Type type)
        {
            Func<object> factory = null;
            bool valueFound = _factoryMap.TryGetValue(type, out factory);
            if (valueFound)
            {
                return factory();
            }

            throw new NotImplementedException($"factory for {type.Name} is not implemented");
        }

        private object GetRandomText()
        {
            return Guid.NewGuid().ToString();
        }

        private object GetRandomNumber()
        {
            return new Random().Next(100, 200);
        }

        private object GetRandomDate()
        {
            return DateTime.Now.AddDays(-(int)GetRandomNumber());
        }

        private object GetRandomChar()
        {
            int num = new Random().Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }

        private object GetRandomBoolean()
        {
            return new Random().Next(2) == 0;
        }
    }
}
