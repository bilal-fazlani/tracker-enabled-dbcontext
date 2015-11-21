using System;
using System.Linq;
using System.Reflection;

namespace TrackerEnabledDbContext.Common.Testing.Code
{
    public class ObjectFiller<TEntity> where TEntity : class
    {
        private Predicate<string> _propertyNameIgnoreRule;
        readonly RandomDataGenerator _randomDataGenerator = new RandomDataGenerator();

        public void IgnorePropertiesWhen(Predicate<string> propertyNameIgnoreRule)
        {
            _propertyNameIgnoreRule = propertyNameIgnoreRule;
        }

        public void Fill(TEntity obj)
        {
            var properties = typeof (TEntity)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToList();

            foreach (var propertyInfo in properties)
            {
                if (!_propertyNameIgnoreRule(propertyInfo.Name))
                {
                    propertyInfo.SetValue(obj, _randomDataGenerator.Get(propertyInfo.PropertyType));
                }
            }
        }
    }
}