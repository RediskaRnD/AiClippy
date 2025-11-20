using Dapper;
using System.Data;

namespace Persistence.Mappers
{
    internal sealed class DateTimeMapper : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = new DateTimeOffset(value).ToUnixTimeSeconds();
        }

        public override DateTime Parse(object value)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)value).UtcDateTime;
        }
    }
}