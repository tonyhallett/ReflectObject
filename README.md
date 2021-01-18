A simple reflection wrapper for objects.
Use case:
Given an object where you have access to the interface but need properies / methods of the underlying class.
Instead of using reflection create a derivation of ReflectObjectProperties and have the reflection done for you.

e.g given the type below where you want to get the current value of Prop1 and Prop2
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
	public SomeType(object actualSomeType):base(someType){}

	public bool Prop1 {get;protected set;}

	[ReflectFlags(BindingFlags.NonPublic|BindingFlags.Instance)]
	public string Prop2 {get;protected set;}
}
```

By default properties are reflected with ```BindingFlags.Public | BindingFlags.Instance``` ( and with the same name ).  Apply the ReflectFlagsAttribute to change this behaviour.

It is also possible to have a tree of objects.  For instance.

```
class SomeType{
	public SomeType(object actualSomeInternalType):base(someInternalType){}

	public SomeChildType Child {get;protected set;}

}

class SomeChildType{
	public bool Prop1 {get;protected set;}
}
```

We can create the wrappers
```
class SomeType: ReflectObjectProperties{
	public SomeType(object actualSomeInternalType):base(someInternalType){}

	public SomeChildType Child {get;protected set;}

}
class SomeChildType: ReflectObjectProperties{
	public SomeChildType(object actualSomeInternalType):base(someInternalType){}

	public bool Prop1 {get;protected set;}
}
```

such that
```
object someType = GetHiddenType();
var someTypeReflected = new SomeType(someType);
var childProp1 = someTypeReflected.Child.Prop1;
```

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
  public SomeType(object someType):base(someType){}

  [ReflectFlags(BindingFlags.NonPublic|BindingFlags.Instance)]
  public Action VoidMethod {get;protected set;}

  public Func<bool,decimal,string> NonVoidMethod {get;protected set;}
}
```

**Again this is limited ( currrently ).  You are limited to methods where you have access to the types.**

Because Func/Action properties of ReflectObjectProperties are treated as methods you need to specify when they are to be treated as properties.
You must apply the ```DelegatePropertyAttribute``` to the property.

**For now do not add any additional public properties on your derivation of ReflectObjectProperties.**
Todo: provide Exclude attribute or do not throw when properties do not exist on the reflected object type.




