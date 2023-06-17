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

def load_documents(pdf_path):
    loader = PyPDFLoader(pdf_path)
    return loader.load_and_split()

def split_text(documents, chunk_size=1000, chunk_overlap=200):
    text_splitter = CharacterTextSplitter(chunk_size=chunk_size, chunk_overlap=chunk_overlap)
    return text_splitter.split_documents(documents)

def setup_pinecone(chunks, model_name, api_key, environment, index_name):
    embeddings = HuggingFaceInstructEmbeddings(model_name=model_name, model_kwargs={"device": "cpu"})
    pinecone.init(api_key=api_key, environment=environment)
    pineconedb = Pinecone.from_documents(documents=chunks, embedding=embeddings, index_name=index_name)
    return pineconedb.as_retriever(search_kwargs={"k": 2})

def setup_retrieval_qa(local_llm, retriever):
    return RetrievalQA.from_chain_type(llm=local_llm, chain_type="stuff", retriever=retriever, return_source_documents=True)

def ask_question(qa_chain, query):
    def process_llm_response(llm_response):
        print(llm_response['result'])
        print('\n\nSources:')
        for source in llm_response["source_documents"]:
            print(source.metadata['source'])
    llm_response = qa_chain(query)
    process_llm_response(llm_response)


local_llm = setup_model('google/flan-t5-base')
documents = load_documents('C:/C# - learning/AI_Documentation/BackEnd/DocuAurora_Python/DocuAurora/Model/Bulgaria.pdf')
documents = load_documents('C:/C# - learning/AI_Documentation/BackEnd/DocuAurora_Python/DocuAurora/Model/Test2.pdf')
chunks = split_text(documents)
retriever = setup_pinecone(chunks, "hkunlp/instructor-xl", 'b4b7947c-96fd-4c95-9785-9c8ced03b64b', 'us-west1-gcp-free', "test2")
qa_chain = setup_retrieval_qa(local_llm, retriever)

# Asking Questions
ask_question(qa_chain, "who have contribution for book history in Bulgaria?")
ask_question(qa_chain, "who is John Draper?")
ask_question(qa_chain, "what is the capital of Germany?")