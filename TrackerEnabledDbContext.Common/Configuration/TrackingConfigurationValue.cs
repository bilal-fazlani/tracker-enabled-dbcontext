namespace TrackerEnabledDbContext.Common.Configuration
{
    public class TrackingConfigurationValue
    {
        public TrackingConfigurationValue(bool value = false, TrackingConfigurationPriority priority = TrackingConfigurationPriority.Default)
        {
            Value = value;
            Priority = priority;
        }

        public bool Value { get;  private set; }
        public TrackingConfigurationPriority Priority { get; private set; }
    }
}
