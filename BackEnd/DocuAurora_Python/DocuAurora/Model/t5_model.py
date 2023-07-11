from langchain.embeddings import HuggingFaceInstructEmbeddings
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer, pipeline, LlamaTokenizer, LlamaForCausalLM
from langchain.document_loaders import PyPDFLoader
from langchain.text_splitter import CharacterTextSplitter
from langchain.llms import HuggingFacePipeline
from langchain.vectorstores.pinecone import Pinecone
from langchain.chains import RetrievalQA
import pinecone


def setup_model_flan(model_id):
    tokenizer = AutoTokenizer.from_pretrained(model_id)
    model = AutoModelForSeq2SeqLM.from_pretrained(model_id)
    pipe = pipeline("text2text-generation", model=model, tokenizer=tokenizer)
    return HuggingFacePipeline(pipeline=pipe)

def setup_model_wizardvicuna(model_id):
    tokenizer = LlamaTokenizer.from_pretrained(model_id)
    model = LlamaForCausalLM.from_pretrained(model_id)

    pipe = pipeline(
        "text-generation",
        model=model,
        tokenizer=tokenizer,
        max_length=2048,
        temperature=0,
        top_p=0.95,
        repetition_penalty=1.15
    )

    return HuggingFacePipeline(pipeline=pipe)


def asking_existing_index(model_name, api_key, environment, index_name):
    embeddings = HuggingFaceInstructEmbeddings(model_name=model_name, model_kwargs={"device": "cpu"})
    pinecone.init(api_key=api_key, environment=environment)
    pineconedb = Pinecone.from_existing_index(index_name=index_name, embedding=embeddings)
    return pineconedb.as_retriever(search_type="similarity", search_kwargs={"k":1})

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
    return answer


# local_llm = setup_model_wizardvicuna('ehartford/Wizard-Vicuna-7B-Uncensored') #chose one
# # local_llm = setup_model_flan('google/flan-t5-base') # chose one
#
# retriever = asking_existing_index("hkunlp/instructor-xl", 'b4b7947c-96fd-4c95-9785-9c8ced03b64b', 'us-west1-gcp-free',
#                                    "test")
# qa_chain = setup_retrieval_qa(local_llm, retriever)
# #
# # # Asking Questions
# ask_question(qa_chain, "who is John Draper?")