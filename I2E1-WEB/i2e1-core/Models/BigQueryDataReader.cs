using Google.Cloud.BigQuery.V2;
using System.Collections.Generic;

namespace i2e1_core.Models
{
    public class BigQueryDataReader
    {
        private BigQueryResults results;

        private BigQueryRow currentRow;

        private IEnumerator<BigQueryRow> iterator;

        private int index = 0;

        public BigQueryDataReader(BigQueryResults results) 
        {
            this.results = results;
        }

        public bool Read()
        {
            if (results.TotalRows > (ulong)index)
            {
                if(index == 0)
                {
                    iterator = results.GetEnumerator();
                }
                iterator.MoveNext();
                currentRow = iterator.Current;
                index++;
                return true;
            }
            return false;
        }

        public object this[string key]
        {
            get
            {
                return currentRow[key];
            }
        }

        public int FieldCount
        {
            get
            {
                return currentRow.Schema.Fields.Count;
            }
        }

        public string GetName(int i)
        {
            return currentRow.Schema.Fields[i].Name;
        }

        public object GetValue(int i)
        {
            return currentRow[i];
        }
    }
}
