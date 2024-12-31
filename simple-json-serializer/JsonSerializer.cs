using System.Collections;
using System.Reflection;
using System.Text;

namespace SimpleJsonSerializer;

public record JsonSerializerSettings(bool UseCamelCase = false, bool IgnoreNullValues = false, bool Indented = false);

public class JsonSerializer
{
    private static readonly HashSet<Type> NumericTypes =
    [
        typeof(int), typeof(long), typeof(short), typeof(byte), typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
    ];

    public static string Serialize<T>(T instance, JsonSerializerSettings? settings = null)
    {
        settings ??= new JsonSerializerSettings();
        if (instance == null) 
            throw new ArgumentNullException(nameof(instance));

        return SerializeObjectInternal(instance, settings);
    }
    
    private static string SerializeObjectInternal<T>(T? instance, JsonSerializerSettings settings)
    {
        if (instance is null) 
            return "null";

        var instanceType = instance.GetType();

        if (instanceType == typeof(string)) 
            return $"\"{instance}\"";

        if (NumericTypes.Contains(instanceType)) 
            return instance.ToString()!;

        if (IsIterable(instanceType))
        {
            var iterableStringBuilder = new StringBuilder();
            iterableStringBuilder.Append("[");

            var list = (IList)instance;

            foreach (var element in list)
                iterableStringBuilder.Append($" {SerializeObjectInternal(element, settings)},");
            
            RemoveTrailingCommas(iterableStringBuilder);
            iterableStringBuilder.Append(" ]");
            
            return iterableStringBuilder.ToString();
        }

        var instanceProperties = instanceType.GetProperties(BindingFlags.Public);
        var instanceFields = instanceType.GetFields(BindingFlags.Public);

        var objectStringBuilder = new StringBuilder();
        objectStringBuilder.Append("{");

        foreach (var instanceProperty in instanceProperties) 
            objectStringBuilder.Append($" \"{GetMemberName(instanceProperty, settings)}\": {SerializeObjectInternal(GetMemberValue(instanceProperty, instance), settings)},");

        foreach (var instanceField in instanceFields) 
            objectStringBuilder.Append($" \"{GetMemberName(instanceField, settings)}\": {SerializeObjectInternal(GetMemberValue(instanceField, instance), settings)},");

        RemoveTrailingCommas(objectStringBuilder);
        objectStringBuilder.Append(" }");
        
        return objectStringBuilder.ToString();
    }

    private static object? GetMemberValue<T>(MemberInfo memberInfo, T instance) =>
        memberInfo switch
        {
            PropertyInfo propertyInfo => propertyInfo.GetValue(instance),
            FieldInfo fieldInfo => fieldInfo.GetValue(instance),
            _ => null
        };

    private static void RemoveTrailingCommas(StringBuilder stringBuilder) => stringBuilder.Replace(",", string.Empty, stringBuilder.Length - 1, 1);

    private static bool IsIterable(Type memberType) => memberType.IsArray || typeof(IList).IsAssignableFrom(memberType);

    private static string GetMemberName(MemberInfo memberInfo, JsonSerializerSettings settings)
    {
        var jsonFieldAttribute = memberInfo.GetCustomAttribute<JsonFieldAttribute>();
        var memberName = jsonFieldAttribute?.Name ?? memberInfo.Name;

        return settings.UseCamelCase ? ToCamelCase(memberName) : memberName;
    }

    private static string ToCamelCase(string memberName)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            return memberName;

        return char.ToLowerInvariant(memberName[0]) + memberName[1..];
    }
}
