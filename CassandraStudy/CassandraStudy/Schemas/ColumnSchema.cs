using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassandraStudy.Schemas
{
    internal class ColumnSchema
    {
        public string KeyspaceName { get; set; }

        public string ColumnFamilyName { get; set; }

        public string ColumnName { get; set; }

        public int ComponentIndex { get; set; }

        public string Validator { get; set; }

        public string IndexName { get; set; }
    }
}
