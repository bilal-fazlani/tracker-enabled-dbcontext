// ReSharper disable once CheckNamespace

using System.Collections;
using System.Collections.Generic;
using TrackerEnabledDbContext.Common.Models;
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Enables tracking of Entity tables.
    ///     Place this attribute on a entity class which you want to track for audit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TrackChangesAttribute : Attribute
    {
        public TrackChangesAttribute(bool trackChnages = true, object changeTypes = null)
        {
            Enabled = trackChnages;

            if (changeTypes != null && changeTypes is IEnumerable)
            {
                ChangeTypes = ((IEnumerable<EventType>)changeTypes);
            }
        }

        public bool Enabled { get; set; }

        public IEnumerable<EventType> ChangeTypes { get; set; }
    }
}