using System;
using System.Linq;
using System.Reflection;

namespace ReflectObject
{
    internal class MethodSetter : PropertySetterBase<MethodInfo>
    {
        private readonly bool isAction;

		public MethodSetter(PropertyInfo ownProperty,bool isAction, BindingFlags bindingFlags, Type reflectedType):base(ownProperty, bindingFlags,reflectedType)
        {
            this.isAction = isAction;
        }
		protected override object GetPropertyValue(ReflectObjectProperties wrapper, object reflectedObject)
        {
			return MethodWrapper.CreateDelegateWrapper(reflectedMember, reflectedObject, ownProperty.PropertyType, isAction);
            
        }

        protected override MethodInfo GetMember(string memberName)
        {
            var genericTypeArguments = ownProperty.PropertyType.GenericTypeArguments;
            var parameterTypes = isAction ? genericTypeArguments : genericTypeArguments.Take(genericTypeArguments.Length - 1).ToArray();
            return reflectedType.GetMethod(memberName, bindingFlags, null, parameterTypes, new ParameterModifier[] { });
        }

        
	}

}
