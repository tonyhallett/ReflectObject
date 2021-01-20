using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectObject
{
    internal class PropertySetter : IPropertySetter
    {
        private readonly PropertyInfo ownProperty;
        private readonly BindingFlags bindingFlags;
        private readonly PropertyInfo reflectedProperty;
		private static Type listType = typeof(List<>);
		private static Type enumerableTTYpe = typeof(IEnumerable<>);
		private static MethodInfo createEnumerableMethodInfo;
		private Func<object, object> coerceValue;
		private Type ownPropertyType;
		private Action<object, object> ownPropertySetter;

		static PropertySetter()
		{
			createEnumerableMethodInfo = typeof(PropertySetter).GetMethod(nameof(CreateEnumerable), BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public PropertySetter(PropertyInfo ownProperty, BindingFlags bindingFlags,Type reflectedType)
        {
            this.ownProperty = ownProperty;
            this.bindingFlags = bindingFlags;
			reflectedProperty = reflectedType.GetProperty(ownProperty.Name, bindingFlags);
			ownPropertyType = ownProperty.PropertyType;
		}
        public void Set(ReflectObjectProperties wrapper, object reflectedObject)
        {
			var value = ReflectedPropertyValue(reflectedObject);
			if (value != null)
			{
				value = CoerceValue(value);
				if (ownPropertySetter == null)
				{
					ownPropertySetter = PropertySetterHelper.BuildSetAccessor(ownProperty.GetSetMethod(true));
				}
				ownProperty.SetValue(wrapper, value);
			}
		}
		internal object Wrap(Type type, object toReflect)
		{
			return Activator.CreateInstance(type, toReflect);
		}
		private T WrapTyped<T>(object toReflect)
		{
			return (T)Wrap(typeof(T), toReflect);
		}
		private MethodInfo GetCreateEnumerableMethodInfo(Type type)
		{
			return createEnumerableMethodInfo.MakeGenericMethod(type);
		}
		private IEnumerable<T> CreateEnumerable<T>(IEnumerable value) where T : ReflectObjectProperties
		{
			var enumerator = (value as IEnumerable).GetEnumerator();
			while (enumerator.MoveNext())
			{

				yield return enumerator.Current == null ? null : WrapTyped<T>(enumerator.Current);
			}
		}
		private object CoerceValue(object value)
		{
			if(coerceValue == null)
            {
				if (ReflectObjectProperties.IsReflectObjectPropertiesType(ownPropertyType))
				{
					coerceValue = v => Wrap(ownPropertyType, v);
				}
				else
				{
					/*
						implement another time - List<> and IEnumerable<> will be sufficient
						if (ownPropertyType.IsArray)
						{
							var elementType = ownPropertyType.GetElementType();
							...

						}
					*/

					// will know path taken so should be able to store - again as a static 
					if (ownPropertyType.IsGenericType)
					{
						var genericArguments = ownPropertyType.GetGenericArguments();
						var genericArgument = genericArguments[0];
						if (genericArguments.Length == 1 && ReflectObjectProperties.IsReflectObjectPropertiesType(genericArgument))
						{
							var genericTypeDefinitionType = ownPropertyType.GetGenericTypeDefinition();

							if (genericTypeDefinitionType == listType)
							{
								var constructedListType = listType.MakeGenericType(genericArgument);
								var addMethod = constructedListType.GetMethod("Add");

								coerceValue = v =>
								{
									var list = Activator.CreateInstance(constructedListType);
									var enumerator = (v as IEnumerable).GetEnumerator();
									while (enumerator.MoveNext())
									{
										addMethod.Invoke(list, new object[] { enumerator.Current == null ? null : Wrap(genericArgument, enumerator.Current) });
									}
									return list;
								};

							}
							else if (genericTypeDefinitionType == enumerableTTYpe)
							{
								var createEnumerableMethodInfo = GetCreateEnumerableMethodInfo(genericArgument);
								coerceValue = v => createEnumerableMethodInfo.Invoke(this, new object[] { v });
							}
						}


					}

				}
				if(coerceValue == null)
                {
					coerceValue = v => v;
				}

            }
			
			return coerceValue(value);
		}
		private object ReflectedPropertyValue(object reflectedObject)
		{
			object value = null;
			
			if (reflectedProperty != null)
			{
				value = reflectedProperty.GetValue(reflectedObject);
			}
			return value;
		}
	}

}
