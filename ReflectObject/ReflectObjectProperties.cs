using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ReflectObject
{
    public abstract class ReflectObjectProperties
	{
		private const BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Public;
		public object ReflectedObject { get; }
		public Type ReflectedType { get; }
		
		//Defined up to 8 parameters
		private static List<Type> funcTypes = new List<Type>
		{
			typeof(Func<>),typeof(Func<,>),typeof(Func<,,>),typeof(Func<,,,>),typeof(Func<,,,,>),typeof(Func<,,,,,>),typeof(Func<,,,,,,>),typeof(Func<,,,,,,,>),typeof(Func<,,,,,,,,>)
		};
		private static List<Type> actionTypes = new List<Type>
		{
			typeof(Action),typeof(Action<>),typeof(Action<,>),typeof(Action<,,>),typeof(Action<,,,>),typeof(Action<,,,,>),typeof(Action<,,,,,>),typeof(Action<,,,,,,>),typeof(Action<,,,,,,,>)
		};
		private static ConcurrentDictionary<Type, List<IPropertySetter>> cache = new ConcurrentDictionary<Type, List<IPropertySetter>>();

		public ReflectObjectProperties(object toReflect)
		{
			ReflectedObject = toReflect;
			ReflectedType = toReflect.GetType();
			var thisType = this.GetType();

			var setters = cache.GetOrAdd(thisType, (_) =>
			{
				return GetOwnSettableProperties().Select(ownProperty =>
				{
					IPropertySetter propertySetter = null;
					var bindingFlags = GetBindingFlags(ownProperty);
					var isAction = FuncActionPropertyCheck(ownProperty.PropertyType);
					if (isAction.HasValue && !FuncActionPropertyIsProperty(ownProperty))
					{
						propertySetter = new MethodSetter(ownProperty, isAction.Value, bindingFlags, ReflectedType);
                    }
                    else
                    {
						propertySetter = new PropertySetter(ownProperty, bindingFlags, ReflectedType);
					}
					return propertySetter;

				}).ToList();
			});
			
			setters.ForEach(setter => setter.Set(this, ReflectedObject));
		}

		private bool FuncActionPropertyIsProperty(PropertyInfo property)
        {
			return property.GetCustomAttribute<DelegatePropertyAttribute>() != null;
        }

		private bool? FuncActionPropertyCheck(Type propertyType)
        {
			if(propertyType == typeof(Action))
            {
				return true;
            }
            if (propertyType.IsGenericType)
            {
				var genericTypeDefinition = propertyType.GetGenericTypeDefinition();
				var funcType = funcTypes.SingleOrDefault(func => func == genericTypeDefinition);
                if (funcType != null)
                {
					return false;
                }
				var actionType = actionTypes.SingleOrDefault(action => action == genericTypeDefinition);
                if (actionType != null)
                {
					return true;
                }
			}
			return null;
        }

		private BindingFlags GetBindingFlags(PropertyInfo ownProperty)
		{
			var bindingFlags = defaultBindingFlags;
			var reflectFlags = ownProperty.GetCustomAttribute<ReflectFlagsAttribute>();
			if (reflectFlags != null)
			{
				bindingFlags = reflectFlags.BindingFlags;
			}
			return bindingFlags;
		}

		private IEnumerable<PropertyInfo> GetOwnSettableProperties()
		{
			var excludeProperties = new List<string> { nameof(ReflectedObject), nameof(ReflectedType) };
			var ownProperties = this.GetType().GetProperties();
			return ownProperties.Where(p => !excludeProperties.Contains(p.Name)).Where(p => p.GetCustomAttribute<IgnoreAttribute>() == null);
		}

		internal static bool IsReflectObjectPropertiesType(Type type)
		{
			return typeof(ReflectObjectProperties).IsAssignableFrom(type);
		}
		
		public static void ClearCache()
		{
			cache = new ConcurrentDictionary<Type, List<IPropertySetter>>();
		}
		public static void ClearCache(Type reflectObjectPropertiesType)
        {
            if (cache.ContainsKey(reflectObjectPropertiesType))
            {
				cache.TryRemove(reflectObjectPropertiesType, out _);
            }
        }

	}

}
