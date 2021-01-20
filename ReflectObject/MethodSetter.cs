using System;
using System.Linq;
using System.Reflection;

namespace ReflectObject
{
    internal class MethodSetter : IPropertySetter
    {
        private readonly PropertyInfo ownProperty;
        private readonly bool isAction;
        private readonly BindingFlags bindingFlags;
        private readonly Type reflectedType;
		private MethodInfo reflectedTypeMethod;
		private Action<object, object> ownPropertySetter;

		public MethodSetter(PropertyInfo ownProperty,bool isAction, BindingFlags bindingFlags, Type reflectedType)
        {
            this.ownProperty = ownProperty;
            this.isAction = isAction;
            this.bindingFlags = bindingFlags;
            this.reflectedType = reflectedType;
        }
		public void Set(ReflectObjectProperties wrapper, object reflectedObject)
        {
			var value = WrapFuncOrAction(reflectedObject);
            if (ownPropertySetter == null)
            {
                ownPropertySetter = PropertySetterHelper.BuildSetAccessor(ownProperty.GetSetMethod(true));
            }
            ownPropertySetter(wrapper, value);
        }
		
		private Delegate WrapFuncOrAction(object reflectedObject)
		{
			if(reflectedTypeMethod == null)
            {
				var genericTypeArguments = ownProperty.PropertyType.GenericTypeArguments;
				var parameterTypes = isAction ? genericTypeArguments : genericTypeArguments.Take(genericTypeArguments.Length - 1).ToArray();
				reflectedTypeMethod = reflectedType.GetMethod(ownProperty.Name, bindingFlags, null, parameterTypes, new ParameterModifier[] { });
			}

			return MethodWrapper.CreateDelegateWrapper(reflectedTypeMethod, reflectedObject, ownProperty.PropertyType, isAction);
		}
	}

}
