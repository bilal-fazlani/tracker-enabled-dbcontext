using System;
using System.Linq.Expressions;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public class TrackAllResponse<T>
    {
        public ExceptResponse<T> Except(Expression<Func<T, object>> property)
        {
            OverrideTrackingResponse<T>.SkipProperty(property);
            return new ExceptResponse<T>();
        }
    }
}