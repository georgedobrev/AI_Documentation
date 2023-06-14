from langchain.embeddings import HuggingFaceInstructEmbeddings
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer, pipeline
from langchain.document_loaders import PyPDFLoader
from langchain.document_loaders import TextLoader
from langchain.document_loaders import DirectoryLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.text_splitter import CharacterTextSplitter
from langchain.vectorstores.pinecone import Pinecone
from langchain.llms import HuggingFacePipeline
from langchain import PromptTemplate,  LLMChain
from langchain.chains import RetrievalQA
import pinecone
import textwrap

def process_llm_response(llm_response):
    print(llm_response['result'])
    print('\n\nSources:')
    for source in llm_response["source_documents"]:
        print(source.metadata['source'])

model_id = 'google/flan-t5-base'
tokenizer = AutoTokenizer.from_pretrained(model_id)
model = AutoModelForSeq2SeqLM.from_pretrained(model_id)

pipe = pipeline(
    "text2text-generation",
    model=model,
    tokenizer=tokenizer
)

local_llm = HuggingFacePipeline(pipeline=pipe)

loader = PyPDFLoader('C:/C# - learning/AI_Documentation/BackEnd/DocuAurora_Python/DocuAurora/Model/Bulgaria.pdf')

documents = loader.load_and_split()

# Split text into chunks
text_splitter = CharacterTextSplitter(chunk_size=1000, chunk_overlap=200)
chunks = text_splitter.split_documents(documents)

embeddings = HuggingFaceInstructEmbeddings(model_name="hkunlp/instructor-xl", model_kwargs={"device": "cpu"})

pinecone.init(api_key='b4b7947c-96fd-4c95-9785-9c8ced03b64b', environment='us-west1-gcp-free')

index_name = "test2"  # replace with your index name
pineconedb = Pinecone.from_documents(documents=chunks, embedding=embeddings, index_name=index_name)

retriever = pineconedb.as_retriever(search_kwargs={"k": 3})

qa_chain = RetrievalQA.from_chain_type(llm=local_llm, chain_type="stuff", retriever=retriever, return_source_documents=True)

query = "what is the capital of Germany? "
llm_response = qa_chain(query)
process_llm_response(llm_response)

