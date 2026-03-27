using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using System.Collections.Generic;
using System.Data;

namespace i2e1_core.Models.Entity
{
    public class RadacctDb : MySqlDbEntity
    {
        public RadacctDb(params LongIdInfo[] longIds) : base("p_radacct", longIds)
        {
            var tableName = GetTableName(PartitionKey.NONE);
            lastTableCreated = tableName;
        }

        private static string lastTableCreated;
    }
}
