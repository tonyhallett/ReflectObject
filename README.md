A simple reflection wrapper for objects.
Use case:
Given an object where you have access to the interface but need properies / methods of the underlying class.
Instead of using reflection create a derivation of ReflectObjectProperties and have the reflection done for you.

e.g given the type below where you want to get the value of Prop1 and Prop2
```
class SomeType{
	public bool Prop1 {get;set;}
	internal string Prop2 {get;set;}
	// other members
}
```

create the following class :
```
class SomeType: ReflectObjectProperties{
	public SomeType(object actualSomeType):base(actualSomeType){}

	public bool Prop1 {get;protected set;}

	[ReflectFlags(BindingFlags.NonPublic|BindingFlags.Instance)]
	public string Prop2 {get;protected set;}
}
```

By default properties are reflected with ```BindingFlags.Public | BindingFlags.Instance``` ( and with the same name ).  Apply the ReflectFlagsAttribute to change this behaviour.
**Any additional public properties on your derivation of ReflectObjectProperties must be ignored by applying the Ignore attribute.**

It is also possible to have a tree of objects.  For instance.

```
class SomeType{
	public SomeChildType Child {get; set;}

}

class SomeChildType{
	public bool Prop1 {get; set;}
}
```

We can create the wrappers
```
class SomeType: ReflectObjectProperties{
	public SomeType(object actualSomeType):base(actualSomeType){}

	public SomeChildType Child {get;protected set;}

}
class SomeChildType: ReflectObjectProperties{
	public SomeChildType(object actualSomeChildType):base(actualSomeChildType){}

	public bool Prop1 {get;protected set;}
}
```

such that
```
object someType = GetHiddenType();
var someTypeReflected = new SomeType(someType);
var childProp1 = someTypeReflected.Child.Prop1;
```

For IEnumerable property types you have two options for wrapping these items.  Use the property types `IEnumerable<ReflectObjectPropertiesDeriv>` or `List<ReflectObjectPropertiesDeriv>`.  
The former will be yielded the latter will immediately enumerate and wrap.
Given
```
class SomeType{
	public IEnumerable<SomeChildType> Children1 {get; set;}
	public IEnumerable<SomeChildType> Children2 {get; set;}

}

class SomeChildType{
}

class SomeType: ReflectObjectProperties{
	public SomeType(object actualSomeType):base(actualSomeType){}

	public IEnumerable<SomeChildType> Children1 {get; set;}
	public List<SomeChildType> Children2 {get; set;}

}
class SomeChildType: ReflectObjectProperties{
	public SomeChildType(object actualSomeChildType):base(actualSomeChildType){}
}

```
Children1 will be yielded and Children2 will immediate wrap the items from the actual enumerable.


**Note that for now property getters return the same value each time.  The value in place upon construction of the reflect object.**
( This code was written for a specific purpose ! )
To do live property getting and setting I would need the introduction of

```
public class ReflectProperty<T>{
	public T GetValue();
	public void SetValue(T value);
}

class SomeType:ReflectObjectProperties{
  public SomeType(object someType):base(someType){}

  public ReflectProperty<string> StringProp {get;protected set;}
}

```


It is also possible to call methods of the reflected object through ReflectObjectProperties.
Provide a property of type Func or Action that matches the signature of the method on the reflected object.

For example given 
```
class SomeType{
	internal void VoidMethod(){
		//....
	}
	public string NonVoidMethod(bool arg1, decimal arg2){
		return "Hello";
	}
}
```

create
```
class SomeType:ReflectObjectProperties{
  public SomeType(object actualSomeType):base(actualSomeType){}

  [ReflectFlags(BindingFlags.NonPublic|BindingFlags.Instance)]
  public Action VoidMethod {get;protected set;}

  public Func<bool,decimal,string> NonVoidMethod {get;protected set;}
}
```

**Again this is limited ( currrently ).  You are limited to methods where you have access to the types.**

Because Func/Action properties of ReflectObjectProperties are treated as methods you need to specify when they are to be treated as properties.
You must apply the ```DelegatePropertyAttribute``` to the property.

Behaviour when the member does not exist on the reflected type :

By default a PropertyDoesNotExistException will be thrown but this can be controlled by adding attributes.
Add DoNotThrowMissingMembers to the class or struct implementing ReflectObjectProperties or 
add DoNotThrowMissingMember to a property.

Finally, there are two properties related to the object being reflected.  ReflectedObject and ReflectedType.




