from langchain.document_loaders import DirectoryLoader
from langchain.text_splitter import CharacterTextSplitter
from langchain.embeddings import HuggingFaceEmbeddings
from langchain.vectorstores.pinecone import Pinecone
from langchain.document_loaders import PyPDFLoader

from pathlib import Path
import pinecone
import asyncio

PINECONE_INDEX_NAME = "<Your_Pinecone_Index_Name>"
PINECONE_NAME_SPACE = "<Your_Pinecone_Namespace>"

async def run():
    
    # Load raw documents from all files in the directory
   
        loader = PyPDFLoader("your_document.pdf")
        documents = loader.load_and_split()

        # Split text into chunks
        text_splitter = CharacterTextSplitter(chunk_size=1000, chunk_overlap=200)

        chunks = text_splitter.split_documents(documents)

        embeddings = HuggingFaceEmbeddings(model, tokenizer)



