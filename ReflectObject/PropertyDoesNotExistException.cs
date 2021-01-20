using System;
using System.Reflection;

namespace ReflectObject
{
    public class PropertyDoesNotExistException : Exception
    {
        public Type ReflectedType { get; }
        public string PropertyName { get; }
        public BindingFlags BindingFlags { get; }

        public PropertyDoesNotExistException(string message, Type reflectedType, string propertyName, BindingFlags bindingFlags) :base(message) {
            ReflectedType = reflectedType;
            PropertyName = propertyName;
            BindingFlags = bindingFlags;
        }

		public static PropertyDoesNotExistException Create(Type reflectedType, string propertyName, BindingFlags bindingFlags)
        {
			var message = $"Property {propertyName} with binding flags {bindingFlags} does not exist on {reflectedType.FullName}";
			return new PropertyDoesNotExistException(message, reflectedType, propertyName, bindingFlags);
        }
		
	}

}
