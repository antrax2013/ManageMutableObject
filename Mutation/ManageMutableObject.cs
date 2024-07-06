using System.Reflection;

namespace Mutation;

public class ManageMutableObject<T>(T? content) : IManageMutableObject<T>
{
    private readonly T? _content = content;

    public T? Content
    {
        get
        {
            if (_content == null)
                return default;
            
            var inst = _content.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (T?)inst?.Invoke(_content, null);
        }
    }

    public K? GetProperty<K>(string propertyName) {
        if (_content == null)
            throw new ArgumentNullException("Nested object is null");

        var propertyInfo = _content.GetType().GetProperty(propertyName);

        return propertyInfo == null
            ? throw new ArgumentNullException($"Property not found {propertyName}")
            : (K?)propertyInfo.GetValue(_content);
    }

    public Result Mute<K>(string propertyName, K value)
    {
        if (_content == null)
            return Result.Fail("Nested object is null");

        var propertyInfo = _content.GetType().GetProperty(propertyName);

        if (propertyInfo == null)
            return Result.Fail($"Property not found {propertyName}");

        try
        {
            propertyInfo.SetValue(_content, value);
        }
        catch (ArgumentException e) {
            return Result.Fail($"Incompatible value for {propertyName} : {e.Message}");
        }

        return Result.Ok();
    }

    public Result Mute(ManageMutableObject<T> src)
    {
        if (src.Content == null)
            return Result.Fail("Nested object of param is null");

        return Mute(src.Content);
    }

    public Result Mute(T src) 
    {
        if (src == null)
            return Result.Fail("Param is null");

        Type srcContentType = src.GetType();
        PropertyInfo[] props = srcContentType.GetProperties();

        foreach (PropertyInfo prop in props) {
            var res = Mute(prop.Name, prop.GetValue(src));
            if (res.IsFailure)
                return res;
        }

        return Result.Ok();
    }
}
