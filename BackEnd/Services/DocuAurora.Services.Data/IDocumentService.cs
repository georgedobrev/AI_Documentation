using DocuAurora.Data.Models.MongoDB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public interface IDocumentService
    {
        List<Document> Get();

        Document Get(string id);

        Document Create(Document student);

        void Update(string id, Document student);

        void Remove(string id);
    }
}
