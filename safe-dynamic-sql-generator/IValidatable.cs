namespace SafeSqlBuilder
{
    public interface IValidatable
    {
        bool IsValid { get; }
        bool IsInvalid => !IsValid;
    }
}