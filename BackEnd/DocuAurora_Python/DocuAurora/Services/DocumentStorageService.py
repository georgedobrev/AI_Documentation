import pinecone
import configparser

class DocumentStorageService:
    def __init__(self, index_name, config_file):
        self.config_file = config_file
        self.index_name = index_name
        self.dimension =  self._get_config_value('Pinecone', 'dimension')

    def _get_api_key(self, section, key):
            config = configparser.ConfigParser()
            config.read(self.config_file)
            return config.get(section, key)

    def create_index(self):
        pinecone.init(api_key=self._get_api_key('Pinecone', 'API_KEY'))
        pinecone.create_index(index_name=self.index_name, dimension=self.dimension)

    def insert_document(self, document_id, embedding):
        pinecone.init(api_key=self._get_api_key())
        index = pinecone.Index(index_name=self.index_name)
        index.upsert(items={document_id: embedding})
        pinecone.deinit()

    def delete_document(self, document_id):
        pinecone.init(api_key=self._get_api_key())
        index = pinecone.Index(index_name=self.index_name)
        index.delete(ids=[document_id])
        pinecone.deinit()