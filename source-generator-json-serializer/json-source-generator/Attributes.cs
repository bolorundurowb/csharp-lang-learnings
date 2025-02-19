using System;

namespace JsonSourceGenerator;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class JsonSerializableAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class JsonIgnoreFieldAttribute : Attribute { }