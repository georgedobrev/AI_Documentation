from langchain.embeddings import HuggingFaceInstructEmbeddings
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer, pipeline
from langchain.document_loaders import PyPDFLoader
from langchain.text_splitter import CharacterTextSplitter
from langchain.llms import HuggingFacePipeline
from langchain.vectorstores.pinecone import Pinecone
from langchain.chains import RetrievalQA
import pinecone


def setup_model(model_id):
    tokenizer = AutoTokenizer.from_pretrained(model_id)
    model = AutoModelForSeq2SeqLM.from_pretrained(model_id)
    pipe = pipeline("text2text-generation", model=model, tokenizer=tokenizer)
    return HuggingFacePipeline(pipeline=pipe)

def asking_existing_index(model_name, api_key, environment, index_name):
    embeddings = HuggingFaceInstructEmbeddings(model_name=model_name, model_kwargs={"device": "cpu"})
    pinecone.init(api_key=api_key, environment=environment)
    pineconedb = Pinecone.from_existing_index(index_name=index_name, embedding=embeddings)
    return pineconedb.as_retriever(search_type="similarity", search_kwargs={"k":3})

def setup_retrieval_qa(local_llm, retriever):
    return RetrievalQA.from_chain_type(llm=local_llm, chain_type="stuff", retriever=retriever, return_source_documents=True)

def ask_question(qa_chain, query):
    def process_llm_response(llm_response):
        return llm_response['result']
        # print('\n\nSources:')
        # for source in llm_response["source_documents"]:
        #     print(source.metadata['source'])

    llm_response = qa_chain(query)
    answer = process_llm_response(llm_response)
    return  answer

   