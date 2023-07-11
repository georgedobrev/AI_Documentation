import configparser
from Model.t5_model import setup_model_flan,setup_model_wizardvicuna , asking_existing_index, setup_retrieval_qa, ask_question
from Data.pinecone_db import load_documents,  split_text, setup_pinecone



class AnswerGeneratorService:
    def __init__(self, config_file):
        self.config_file = config_file
        self.local_llm_FlanT5 = setup_model_flan('google/flan-t5-base')
        self.local_llm_Wizzard = setup_model_wizardvicuna('ehartford/Wizard-Vicuna-7B-Uncensored')
        self.final_local_llm = None
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
        
    def load_existing_index(self, inputModel):
        self.retriever = asking_existing_index(self.model_namePinecone,
                                               self.api_key,
                                               self.environment,
                                               self.index_name)

        if inputModel == 'FlanT5':
            self.final_local_llm = self.local_llm_FlanT5
        elif inputModel == 'Wizzard':
            self.final_local_llm = self.local_llm_Wizzard

        self.qa_chain = setup_retrieval_qa(self.final_local_llm, self.retriever)
        

    def generate_answer(self, question):
        return ask_question(self.qa_chain, question)
