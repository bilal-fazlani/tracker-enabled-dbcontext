# Configuration

In order to track entities, you will have to specify which entities you want to track. You can further specify which properties to track but it is optional. If you don't specify which properties to track, all properties of that entity will be tracked.

You can specify your tracking requirements in 3 ways.

1. Annotations
2. Fluent API
3. Combination of both

With the recent introduction of Fluent API, it gives you more power to change/enable/disable tracking even on runtime.

## Annotations

The following is an example of Comment class

```c#
[TrackChanges]
public class Comment
{
    public int Id { get; set; }

    [SkipTracking]
    public string Text { get; set; }

    public virtual int ParentBlogId { get; set; }

    public virtual Blog ParentBlog { get; set; }
}
```

By putting the annotation [TrackChanges], you specify that this entity should be tracked. But if you don't want to track the Text property, just add the annotation [SkipTracking].

This entity will have 3 columns in table. Id, Text and ParentBlogId. Although entity framework works even if you don't have the property 'ParentBlogId', this library will require you to have it if you wish to track foreign keys.

## Fluent API

If you don't like to put attributes on your entities, you can use the fluent api to configure tracking as following example.

```c#
EntityTracker
    .TrackAllProperties<NormalModel>()
    .Except(x => x.Description)
    .And(x => x.Id);
```

!!! Note
    Note that if you use both, annotations and fluent api, and they are conflicting for an entity or property, fluent api configuration will be considered high priority

Let's consider the following example where you have already configured tracking of a model with either annotations or fluent api. However now on runtime you want to override it for a specific property.

```c#
EntityTracker
    .OverrideTracking<TrackedModelWithMultipleProperties>()
    .Disable(x => x.StartDate);
```

This way, all configuration is maintained and only StartDate tracking is disabled. You can enable it again using Enable() method.

You can configure tracking at 3 levels.

1. Global
2. Entity
3. Property

### Global Level example

Global level tracking is on by default but you can disable it at runtime as follows:

    GlobalTrackingConfig.Enabled = false;

### Entity Level example

Its very similar to Property level configuration -

for overriding entity level configuration,

```c#
EntityTracker
    .TrackAllProperties<TrackedModelWithMultipleProperties>()
    .Except(x => x.Name)
    .And(x => x.Description);

EntityTracker
    .OverrideTracking<TrackedModelWithMultipleProperties>()
    .Disable();

EntityTracker
    .OverrideTracking<TrackedModelWithMultipleProperties>()
    .Enable();
```

In the above example,
you specified tracking configuration for an entity and its properties. Then override the entity level tracking to be disabled **while maintaining the tracking configuration** and then enabled it again.

!!! Note
    While working with overrides, if you don't specify property, they work on entity on entity level