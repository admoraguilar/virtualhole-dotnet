using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace VirtualHole.DB
{
	/// <summary>
	/// Inspired from source: https://stackoverflow.com/a/39613579
	/// </summary>
	public class ReadOnlyPropertyConvention : ConventionBase, IClassMapConvention
	{
		private readonly BindingFlags _bindingFlags;

		public ReadOnlyPropertyConvention()
				: this(BindingFlags.Instance | BindingFlags.Public)
		{ }

		public ReadOnlyPropertyConvention(BindingFlags bindingFlags)
		{
			_bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
		}

		public void Apply(BsonClassMap classMap)
		{
			List<PropertyInfo> readOnlyProperties = classMap.ClassType.GetTypeInfo()
				.GetProperties(_bindingFlags)
				.Where(p => IsReadOnlyProperty(classMap, p))
				.ToList();
			readOnlyProperties.ForEach(p => classMap.MapMember(p));
		}

		private static bool IsReadOnlyProperty(BsonClassMap classMap, PropertyInfo propertyInfo)
		{
			// We can't read 
			if(!propertyInfo.CanRead) { return false; }

			// We can write (already handled by the default convention...)
			if(propertyInfo.CanWrite) { return false; }

			// Skip indexers
			if(propertyInfo.GetIndexParameters().Length != 0) { return false; }

			// Skip overridden properties (they are already included by the base class)
			var getMethodInfo = propertyInfo.GetMethod;
			if(getMethodInfo.IsVirtual && getMethodInfo.GetBaseDefinition().DeclaringType != classMap.ClassType) { return false; }

			return true;
		}
	}
}
