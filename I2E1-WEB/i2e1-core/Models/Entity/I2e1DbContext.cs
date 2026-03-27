using i2e1_basics.Database;
using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace i2e1_core.Models.Entity
{
    public class DbEntity
    {
        public DbEntity(string originalTableName, LongIdInfo[] shardIds)
        {
            this.originalTableName = originalTableName;
            this.shardIds = shardIds;
        }

        public string originalTableName;
        private LongIdInfo[] shardIds;

        public string prepareQuery(PartitionKey partitionKey, string query)
        {
            return query.Replace("{{table_name}}", GetTableName(partitionKey));
        }

        public string GetTableName(PartitionKey partitionKey)
        {
            return originalTableName + partitionKey.Key;
        }

        protected Object ExecuteAll(PartitionKey partitionKey, string query, ExecuteAllResponseHandler resHandler, object param = null, CommandType commandType = CommandType.Text)
        {
            return new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(prepareQuery(partitionKey, query));
                cmd = ShardHelper.FillParametersFromObject(cmd, param);
                cmd.CommandType = commandType;
                res = ResponseType.READER;
                return cmd;
            }),
            new ExecuteAllResponseHandler((reader, shardId) =>
            {
                resHandler(reader, shardId);
            })).ExecuteAll();
        }

        private Object Execute(PartitionKey partitionKey, string query, ExecuteAllResponseHandler resHandler, object param, string tableName)
        {
            return new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(prepareQuery(partitionKey, query));
                cmd = ShardHelper.FillParametersFromObject(cmd, param);
                cmd.CommandType = CommandType.Text;
                res = ResponseType.READER;
                return cmd;
            }),
            new ExecuteAllResponseHandler((reader, shardId) =>
            {
                resHandler(reader, shardId);
            })).ExecuteAll(shardIds, tableName);
        }

        public void ExecutePartition(string query, ExecuteAllResponseHandler resHandler, object param, string tableName, params PartitionKey[] partitionKeys)
        {
            foreach (var partitionKey in partitionKeys)
            {
                this.Execute(partitionKey, query, resHandler, param, tableName);
            }
        }

        public void ExecuteAutoPartition(string query, ExecuteAllResponseHandler resHandler, string tableName, object param, DateTime startDate, DateTime endDate)
        {
            List<PartitionKey> partitionList = new List<PartitionKey>();
            DateTime now = CoreUtil.ConvertUtcToIST(DateTime.UtcNow);
            DateTime counter = startDate;
            while (((counter.Year * 100) + counter.Month <= (endDate.Year * 100) + endDate.Month))
            {
                if (((counter.Year * 100) + counter.Month >= (PartitionKey.PartitionStart.Year * 100) + PartitionKey.PartitionStart.Month))
                    if (((counter.Year * 100) + counter.Month <= (now.Year * 100) + now.Month))
                        partitionList.Add(new PartitionKey() { Key = "_" + counter.Year + "_" + counter.Month });
                counter = counter.AddMonths(1);
            }
            foreach (var partitionKey in partitionList)
            {
                this.Execute(partitionKey, query, resHandler, param, tableName);
            }
        }
    }

    public class MySqlDbEntity
    {
        public MySqlDbEntity(string originalTableName, LongIdInfo[] shardIds)
        {
            this.originalTableName = originalTableName;
            this.shardIds = shardIds;
        }

        public string originalTableName;
        private LongIdInfo[] shardIds;

        public string prepareQuery(PartitionKey partitionKey, string query)
        {
            return query.Replace("{{table_name}}", GetTableName(partitionKey));
        }

        public string GetTableName(PartitionKey partitionKey)
        {
            return originalTableName + partitionKey.Key;
        }

        

        protected Object ExecuteAll(PartitionKey partitionKey, string query, MySqlExecuteAllResponseHandler resHandler, object param = null, CommandType commandType = CommandType.Text)
        {
            return new MySQLQueryExecutor<bool>(new GetMySqlCommand((out ResponseType res) =>
            {
                MySqlCommand cmd = new MySqlCommand(prepareQuery(partitionKey, query));
                cmd = ShardHelper.FillParametersFromObject(cmd, param);
                cmd.CommandType = commandType;
                res = ResponseType.READER;
                return cmd;
            }),
            new MySqlExecuteAllResponseHandler((reader, shardId) =>
            {
                resHandler(reader, shardId);
            })).ExecuteAll();
        }


        private Object Execute(PartitionKey partitionKey, string query, MySqlExecuteAllResponseHandler resHandler, object param, string listName)
        {
            return new MySQLQueryExecutor<bool>(new GetMySqlCommand((out ResponseType res) =>
            {
                MySqlCommand cmd = new MySqlCommand(prepareQuery(partitionKey, query));
                cmd = ShardHelper.FillParametersFromObject(cmd, param);
                cmd.CommandType = CommandType.Text;
                res = ResponseType.READER;
                return cmd;
            }),
            new MySqlExecuteAllResponseHandler((reader, shardId) =>
            {
                resHandler(reader, shardId);
            })).ExecuteAll(shardIds, listName);
        }

        public void ExecutePartition(string query, MySqlExecuteAllResponseHandler resHandler, object param, string tableName, params PartitionKey[] partitionKeys)
        {
            foreach (var partitionKey in partitionKeys)
            {
                this.Execute(partitionKey, query, resHandler, param, tableName);
            }
        }

        public void ExecuteAutoPartition(string query, MySqlExecuteAllResponseHandler resHandler, string listName, object param, DateTime startDate, DateTime endDate)
        {
            List<PartitionKey> partitionList = new List<PartitionKey>();
            DateTime now = CoreUtil.ConvertUtcToIST(DateTime.UtcNow);
            DateTime counter = startDate;
            while (((counter.Year * 100) + counter.Month <= (endDate.Year * 100) + endDate.Month))
            {
                if (((counter.Year * 100) + counter.Month >= (PartitionKey.PartitionStart.Year * 100) + PartitionKey.PartitionStart.Month))
                    if (((counter.Year * 100) + counter.Month <= (now.Year * 100) + now.Month))
                        partitionList.Add(new PartitionKey() { Key = "_" + counter.Year + "_" + counter.Month });
                counter = counter.AddMonths(1);
            }
            foreach (var partitionKey in partitionList)
            {
                this.Execute(partitionKey, query, resHandler, param, listName);
            }
        }
    }
}
