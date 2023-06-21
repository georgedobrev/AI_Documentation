from langchain.embeddings import HuggingFaceInstructEmbeddings
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer, pipeline
from langchain.document_loaders import PyPDFLoader
from langchain.text_splitter import CharacterTextSplitter
from langchain.llms import HuggingFacePipeline
from langchain.vectorstores.pinecone import Pinecone
from langchain.chains import RetrievalQA
import pinecone

def load_documents(pdf_path):
    loader = PyPDFLoader(pdf_path)
    return loader.load_and_split()

def split_text(documents, chunk_size=1000, chunk_overlap=200):
    text_splitter = CharacterTextSplitter(chunk_size=chunk_size, chunk_overlap=chunk_overlap)
    return text_splitter.split_documents(documents)

def setup_pinecone(chunks, model_name, api_key, environment, index_name):
    embeddings = HuggingFaceInstructEmbeddings(model_name=model_name, model_kwargs={"device": "cpu"})
    pinecone.init(api_key=api_key, environment=environment)
    Pinecone.from_documents(documents=chunks, embedding=embeddings, index_name=index_name)

