from langchain.document_loaders import DirectoryLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.embeddings. import OpenAIEmbeddings
from langchain.vectorstores.pinecone import Pinecone
from langchain.document_loaders import PyPDFLoader

from pathlib import Path
import pinecone
import asyncio

PINECONE_INDEX_NAME = "<Your_Pinecone_Index_Name>"
PINECONE_NAME_SPACE = "<Your_Pinecone_Namespace>"

async def run():
    try:
        # Load raw documents from all files in the directory
        file_path = Path("docs")
        directory_loader = DirectoryLoader(file_path, {".pdf": PyPDFLoader})

        raw_docs = await directory_loader.load()

        # Split text into chunks
        text_splitter = RecursiveCharacterTextSplitter(chunk_size=1000, chunk_overlap=200)

        docs = await text_splitter.split_documents(raw_docs)
        print('Split docs:', docs)

        print('Creating vector store...')
        # Create and store the embeddings in the vectorStore
        embeddings = OpenAIEmbeddings()

        index = pinecone.Index(PINECONE_INDEX_NAME)

        # Embed the PDF documents
        await Pinecone.from_documents(docs, embeddings, {
            "pinecone_index": index,
            "namespace": PINECONE_NAME_SPACE,
            "text_key": "text",
        })
    except Exception as error:
        print('Error:', error)
        raise Exception('Failed to ingest your data')

# Run the script
asyncio.run(run())
print('Ingestion complete.')