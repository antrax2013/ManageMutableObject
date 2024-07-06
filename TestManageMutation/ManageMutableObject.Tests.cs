using Mutation;
using NUnit.Framework.Internal;

namespace TestManageMutation;

public class ManageMutableObjectTests
{
    [Test]
    public void When_A_ManageMutableMutant_Is_Created_Then_Porperties_Are_Correctly_Set() {
        ManageMutableObject<Mutant> Thor = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender ='M' });

        Assert.Multiple(() =>
        {
            Assert.That(Thor.Content, Is.Not.Null);
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.
            Assert.That(Thor.Content.Gender, Is.EqualTo('M'));
#pragma warning restore CS8602 // Déréférencement d'une éventuelle référence null.
            Assert.That(Thor.Content.Age, Is.EqualTo(1000));
            Assert.That(Thor.Content.Name, Is.EqualTo("Thor"));
            Assert.That(Thor.Content.Description, Is.EqualTo("God of thunder"));
        });
    }

    [Test]
    public void When_A_ManageMutablNull_Is_Created_Then_Content_Is_Null()
    {
        ManageMutableObject<Mutant> Null = new(null);

        Assert.That(Null.Content, Is.Null);
    }

    [Test]
    public void When_A_ManageMutablEmptyString_Is_Created_Then_Content_Is_NullOrEmpty()
    {
        ManageMutableObject<string> MyString = new(string.Empty);

        Assert.That(string.IsNullOrWhiteSpace(MyString.Content), Is.True);
    }

    [Test]
    public void When_A_ManageMutablEmptyString_Is_Created_Then_String_Properties_Are_Avaliable()
    {
        ManageMutableObject<string> MyString = new(string.Empty);

        Assert.That(MyString.GetProperty<int>("Length"), Is.EqualTo(0));
    }

    [Test]
    public void When_A_Not_Avaliable_Property_Is_Access_Then_An_ArgumentNullException_Fired()
    {
        ManageMutableObject<string> MyString = new(string.Empty);

        Assert.Throws<ArgumentNullException>(() => MyString.GetProperty<int>("anUknownProperty"));
    }

    [Test]
    public void When_Access_To_Propety_Of_Null_Object_Then_An_ArgumentNullException_Fired()
    {
        ManageMutableObject<object> NullObject = new(null);

        Assert.Throws<ArgumentNullException>(() => NullObject.GetProperty<int>("anUknownProperty"));
    }

    [Test]
    public void When_A_ManageMutableMutant_Is_Created_Then_Proporties_Are_Avaliable_Via_GetPropertyAccessor()
    {
        ManageMutableObject<Mutant> Thor = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });

        Assert.Multiple(() =>
        {
            Assert.That(Thor.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Thor.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Thor.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Thor.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }

    [Test]
    public void When_A_Content_Properties_Are_Modified_The_Object_Is_Not_Mutated()
    {
        ManageMutableObject<Mutant> Thor = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        var content = Thor.Content;
        content = new() { Name = "Black Widow", Description = "Russian killer", Age = 35, Gender = 'F' };

        Assert.Multiple(() =>
        {
            Assert.That(Thor.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Thor.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Thor.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Thor.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }
        

    [Test]
    public void When_The_Mute_Method_Is_Called_The_Object_Mutes()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        var content = Avenger.Content;
        content = new() { Name = "Black Widow", Description = "Natasha Romanoff", Age = 35, Gender = 'F' };

        Avenger.Mute(content);

        Assert.Multiple(() =>
        {
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo(content.Gender));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(content.Age));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo(content.Name));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo(content.Description));
        });
    }

    [Test]
    public void When_The_Null_Is_Provided_To_Mute_Method_The_Mutation_Is_Failure()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        Mutant NullObject = null;
#pragma warning disable CS8604 // Existence possible d'un argument de référence null.
        var result = Avenger.Mute(NullObject);
#pragma warning restore CS8604 // Existence possible d'un argument de référence null.

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo($"Param is null"));
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }

    [Test]
    public void When_A_Property_Is_Muted_Then_Object_Property_Is_Mutued_And_Mutation_Is_Ok()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        var result = Avenger.Mute("Description", "The new god of thunder");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("The new god of thunder"));
        });
    }

    [Test]
    public void When_A_Null_Object_Property_Is_Muted_Then_Mutation_Is_Failure()
    {
        ManageMutableObject<Mutant> Avenger = new(null);
        var result = Avenger.Mute("Description", "The new god of thunder");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo($"Nested object is null"));
        });
    }

    [Test]
    public void When_A_Unknown_Property_Is_Muted_Then_Mutation_Is_Failure()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        var result =  Avenger.Mute("UnknownProperty", "The new god of thunder");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo($"Property not found UnknownProperty"));
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }

    [Test]
    public void When_A_Null_Value_Is_Provided_To_Property_Then_Mutation_Is_Success()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        var result = Avenger.Mute<string?>("Name", null);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.Null);
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }

    [Test]
    public void When_A_Incompatible_Value_Is_Provided_For_A_Property_Then_Mutation_Is_Failure()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        var result = Avenger.Mute("Age", "The new god of thunder");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error.StartsWith("Incompatible value for Age"),Is.True);
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }

    [Test]
    public void When_A_ManageMutableObject_Mute_From_Another_ManageMutableObject_Then_Mutation_Is_Success()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        ManageMutableObject<Mutant> Avenger2 = new(new Mutant() { Name = "Black Widow", Description = "Natasha Romanoff", Age = 35, Gender = 'F' });
        
        var result = Avenger.Mute(Avenger2);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('F'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(35));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo("Black Widow"));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("Natasha Romanoff"));
        });
    }


    [Test]
    public void When_A_ManageMutableObject_Mute_From_NullObject_Then_Mutation_Is_Failure()
    {
        ManageMutableObject<Mutant> Avenger = new(new Mutant() { Name = "Thor", Description = "God of thunder", Age = 1000, Gender = 'M' });
        ManageMutableObject<Mutant> Avenger2 = new(null);

        var result = Avenger.Mute(Avenger2);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Nested object of param is null"));
            Assert.That(Avenger.GetProperty<char?>("Gender"), Is.EqualTo('M'));
            Assert.That(Avenger.GetProperty<int>("Age"), Is.EqualTo(1000));
            Assert.That(Avenger.GetProperty<string>("Name"), Is.EqualTo("Thor"));
            Assert.That(Avenger.GetProperty<string>("Description"), Is.EqualTo("God of thunder"));
        });
    }
}