import configparser
from Model.t5_model import setup_model, load_documents, setup_pinecone, setup_retrieval_qa, split_text, ask_question



class AnswerGeneratorService:
    def __init__(self, config_file):
        self.config_file = config_file
        self.local_llm = setup_model('google/flan-t5-base')
        self.documents = []
        self.chunks = []
        self.model_namePinecone = self._get_config_value('Pinecone', 'model_name')
        self.api_key = self._get_config_value('Pinecone', 'API_KEY')
        self.environment = self._get_config_value('Pinecone', 'environment')
        self.index_name = self._get_config_value('Pinecone', 'index_name')
        self.retriever = None
        self.qa_chain = None

    def _get_config_value(self, section, key):
        config = configparser.ConfigParser()
        config.read(self.config_file)
        return config.get(section, key)

    def load_documents(self, document_paths):
        for path in document_paths:
            document = load_documents(path)
            self.documents.append(document)
        self.chunks = split_text(self.documents)
        self.retriever = setup_pinecone(self.chunks,
                                        self.model_namePinecone,
                                        self.api_key,
                                        self.environment,
                                        self.index_name)
        self.qa_chain = setup_retrieval_qa(self.local_llm, self.retriever)

    def generate_answer(self, question):
        return ask_question(self.qa_chain, question)
