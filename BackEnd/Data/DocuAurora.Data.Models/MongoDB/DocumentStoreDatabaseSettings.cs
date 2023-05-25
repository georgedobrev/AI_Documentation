using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Data.Models.MongoDB
{
    public class DocumentStoreDatabaseSettings : IDocumentStoreDatabaseSettings
    {
        public string DocumentCollectionName { get; set; } = String.Empty;

        public string ConnectionString { get; set; } = String.Empty;

        public string DatabaseName { get; set; } = String.Empty;
    }
}
