using i2e1_basics.Utilities;
using System.Data;

namespace i2e1_core.Models.Entity
{
    public class UserActivityDb : DbEntity
    {
        public UserActivityDb(params LongIdInfo[] longIds) : base("p_user_activity", longIds)
        {
            var tableName = GetTableName(PartitionKey.CURRENT_MONTH);
            if (tableName != lastTableCreated)
            {
                this.ExecuteAll(PartitionKey.CURRENT_MONTH, "util_ScriptTable", (reader, shardId) =>
                {
                }, new { schema = "dbo", TableName = originalTableName, NewTableSchema = "dbo", NewTableName = tableName }, CommandType.StoredProcedure);
                lastTableCreated = tableName;
            }
        }

        private static string lastTableCreated;
    }
}
