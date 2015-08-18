using System;
using System.Deployment.Internal;
using System.Linq.Expressions;
using TrackerEnabledDbContext.Common.Extensions;

namespace TrackerEnabledDbContext.Common.Fluent
{
    public class TrackerColumnConfigurator<T> where T : class
    {
        internal TrackerColumnConfigurator(){}

        public TrackerColumnConfigurator<T> SkipTrackingForColumn(Expression<Func<T, object>> propertyExpression)
        {
            TypeExtensions.SkipTrackingFor(propertyExpression);
            return this;
        }
    }
}