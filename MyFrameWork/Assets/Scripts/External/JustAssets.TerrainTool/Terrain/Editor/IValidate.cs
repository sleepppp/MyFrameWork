namespace JustAssets.TerrainUtility
{
    public interface IValidate<T>
    {
        T[] Selection { get; set; }

        bool IsValid(out string reason);
    }
}