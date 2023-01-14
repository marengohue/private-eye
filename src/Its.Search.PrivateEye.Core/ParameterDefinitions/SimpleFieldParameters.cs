namespace Its.Search.PrivateEye.Core.ParameterDefinitions;

public class SimpleFieldParameters<TField>
{
    public static implicit operator TField(SimpleFieldParameters<TField> _) => default;
}
