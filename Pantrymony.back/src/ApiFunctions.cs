using Pantrymony.back.Model;

namespace Pantrymony.back.src;

internal class ApiFunctions
{
    public static IEnumerable<Victual> GetVictuals()
    {
        return DataSource.Data.ToArray();
    }
}