from langchain.embeddings import HuggingFaceEmbeddings
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer
from langchain.document_loaders import PyPDFLoader
from langchain.text_splitter import CharacterTextSplitter
from langchain.vectorstores.pinecone import Pinecone
import pinecone

model_id = 'google/flan-t5-base'
tokenizer = AutoTokenizer.from_pretrained(model_id)
model = AutoModelForSeq2SeqLM.from_pretrained(model_id)

def run():
    loader = PyPDFLoader('C:/C# - learning/AI_Documentation/BackEnd/DocuAurora_Python/DocuAurora/Model/temporary.pdf')

    documents = loader.load_and_split()

    # Split text into chunks
    text_splitter = CharacterTextSplitter(chunk_size=1000, chunk_overlap=200)
    chunks = text_splitter.split_documents(documents)
   
    embeddings = HuggingFaceEmbeddings(model_name=model_id)
    
    pinecone.init(api_key='b4b7947c-96fd-4c95-9785-9c8ced03b64b', environment='us-west1-gcp-free')

    index_name = "test"  # replace with your index name
    docsearch = Pinecone.from_documents(chunks, embeddings, index_name=index_name)

    query = "Will had known they would drag him into the quarrel sooner or later"  # replace with your query
    docs = docsearch.similarity_search(query)
    print(docs[0].page_content)

# Call the function
run()