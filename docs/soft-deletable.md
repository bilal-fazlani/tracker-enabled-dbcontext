Version 3.4 introduces support for soft deletion on entities. 

Suppose you have an interface/abstract class/base class which has "IsDeleted" property or **_ANY bool_** property which indicated that this entity is virtually deleted.

Then you can use this method to set it up. Just run it once on your application startup. 

```c#
GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
(entity => entity.IsDeleted);
```

!!! Note
    - `ISoftDeletable` from the above example can be an interface or any base class.
    - It needs to be created by you and won't be provided in this tracking library
    - `IsDeleted` from above example can have any name but need to be a bool property with public getter and setter

## Soft-Delete Behaviour

 - If you change the soft-delete property from `false` to `true` and save that entity, the event type saved in audit log will be `SoftDeleted`.
 - Important: If you change multiple properties including soft-delete property, it will still be detected and saved as `SoftDeleted` instead of `Modified`.

## Un-Delete Behaviour

Un-deletion works the same way you would expect it to work; change soft delete property from `true` to `false` and log will be saved as `Undeleted`


 
