namespace Mutation;

//https://refactoring.guru/fr/design-patterns/proxy
//https://refactoring.guru/fr/design-patterns/proxy/csharp/example

public interface IManageMutableObject<T> {
    public K? GetProperty<K>(string propertyName);
    public Result Mute<K>(string propertyName, K value);
    public Result Mute(T src);
}
