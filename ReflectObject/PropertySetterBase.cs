using System;
using System.Reflection;

namespace ReflectObject
{
    internal abstract class PropertySetterBase<TMember> : IPropertySetter where TMember:MemberInfo
    {
        protected readonly PropertyInfo ownProperty;
        protected readonly BindingFlags bindingFlags;
        protected readonly Type ownPropertyType;
		protected TMember reflectedMember;
		protected readonly Type reflectedType;
		private Action<object, object> ownPropertySetter;
		private Action<object,object> OwnPropertySetter
        {
            get
            {
				if (ownPropertySetter == null)
				{
					ownPropertySetter = PropertySetterHelper.BuildSetAccessor(ownProperty.GetSetMethod(true));
				}
				return ownPropertySetter;
			}
        }
		private bool throwOnMissingMember;
		public PropertySetterBase(PropertyInfo ownProperty, BindingFlags bindingFlags, Type reflectedType)
		{
			this.ownProperty = ownProperty;
			this.bindingFlags = bindingFlags;
			this.reflectedType = reflectedType;
			ownPropertyType = ownProperty.PropertyType;
			SetThrowOnMissingMember();
		}

		private void SetThrowOnMissingMember()
        {
			var doNotThrowMissingMembersAttribute = ownProperty.DeclaringType.GetCustomAttribute<DoNotThrowMissingMembersAttribute>();
			if(doNotThrowMissingMembersAttribute != null)
            {
				throwOnMissingMember = false;
            }
            else
            {
				var doNotThrowMissingMemberAttribute = ownProperty.GetCustomAttribute<DoNotThrowMissingMemberAttribute>();
				throwOnMissingMember = doNotThrowMissingMemberAttribute == null;
            }
        }
		

		public void Set(ReflectObjectProperties wrapper, object reflectedObject)
        {
			var memberExists = MemberExists();
            if (memberExists)
            {
				var value = GetPropertyValue(wrapper, reflectedObject);
				if (value != null)
				{
					OwnPropertySetter(wrapper, value);
				}
			}
			
		}

		protected abstract TMember GetMember(string memberName);
		
		protected abstract object GetPropertyValue(ReflectObjectProperties wrapper, object reflectedObject);

        private bool MemberExists()
        {
			var memberName = ownProperty.Name;
			reflectedMember = GetMember(memberName);
			if (reflectedMember == null && throwOnMissingMember)
			{
				throw PropertyDoesNotExistException.Create(reflectedType, memberName, bindingFlags);
			}
			return reflectedMember != null;
		}
    }

}
