import pinecone
import configparser


class DocumentStorageService:
    def __init__(self, index_name, config_file, dimension):
        self.config_file = config_file
        self.index_name = index_name
        self.api_key = self._get_config_value('Pinecone', 'API_KEY')
        self.dimension = dimension

    def _get_config_value(self, section, key):
        config = configparser.ConfigParser()
        config.read(self.config_file)
        return config.get(section, key)

    def _initialize_pinecone(self):
        pinecone.init(api_key=self.api_key,environment=self._get_config_value('Pinecone', 'environment'),host="us-west4-gcp-free.pinecone.io")

    def _deinitialize_pinecone(self):
        pinecone.deinit()

    def create_index(self):
        self._initialize_pinecone()
        pinecone.create_index(self.index_name, 3)
        self._deinitialize_pinecone()

    def insert_document(self, document_id, embedding):
        self._initialize_pinecone()
        index = pinecone.Index(index_name=self.index_name)
        index.upsert(items={document_id: embedding})
        self._deinitialize_pinecone()

    def delete_document(self, document_id):
        self._initialize_pinecone()
        index = pinecone.Index(index_name=self.index_name)
        index.delete(ids=[document_id])
        self._deinitialize_pinecone()