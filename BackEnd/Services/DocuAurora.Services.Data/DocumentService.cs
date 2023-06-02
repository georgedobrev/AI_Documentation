using DocuAurora.Data.Models.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class DocumentService 
    {
        private IMongoCollection<Document> _documents;

        public DocumentService(IDocumentStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            this._documents = database.GetCollection<Document>(settings.DocumentCollectionName);
        }

        public Document Create(Document document)
        {
            this._documents.InsertOne(document);
            return document;
        }

        public List<Document> Get()
        {
            return this._documents.Find(document => true).ToList();
        }

        public Document Get(string id)
        {
            return this._documents.Find(document => document.Id == id).FirstOrDefault();
        }

        public void Remove(string id)
        {
            this._documents.DeleteOne(document => document.Id == id);
        }

        public void Update(string id, Document document)
        {
            this._documents.ReplaceOne(document => document.Id == id, document);
        }
    }
}
