using System;

namespace json_source_generator
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonSerializableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonIgnoreAttribute : Attribute
    {
    }
}