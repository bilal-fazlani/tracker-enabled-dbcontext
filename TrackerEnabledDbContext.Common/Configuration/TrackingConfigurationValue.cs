namespace TrackerEnabledDbContext.Common.Configuration
{
    internal class TrackingConfigurationValue
    {
        internal TrackingConfigurationValue(bool value = false, TrackingConfigurationPriority priority = TrackingConfigurationPriority.Default)
        {
            Value = value;
            Priority = priority;
        }

        internal bool Value { get;  private set; }
        internal TrackingConfigurationPriority Priority { get; private set; }
    }
}
