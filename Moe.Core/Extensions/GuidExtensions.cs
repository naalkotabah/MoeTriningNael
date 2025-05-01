namespace Moe.Core.Extensions;

public static class GuidExtensions
{
    public static bool IsNullOrZeros(this Guid? id)
    {
        return id == null || Guid.Empty.Equals(id);
    }
}