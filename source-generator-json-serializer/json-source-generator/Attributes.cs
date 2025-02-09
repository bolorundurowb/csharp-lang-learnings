using System;

namespace json_source_generator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonSerializableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonIgnoreFieldAttribute : Attribute
    {
    }
}