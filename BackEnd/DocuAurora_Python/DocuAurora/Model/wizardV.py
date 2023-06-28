
from transformers import LlamaTokenizer, LlamaForCausalLM, GenerationConfig, pipeline
from langchain.embeddings import HuggingFaceInstructEmbeddings
from langchain.llms import HuggingFacePipeline
from langchain.vectorstores.pinecone import Pinecone
from langchain.chains import RetrievalQA
from InstructorEmbedding import INSTRUCTOR
import pinecone


tokenizer = LlamaTokenizer.from_pretrained("ehartford/Wizard-Vicuna-7B-Uncensored")

model = LlamaForCausalLM.from_pretrained("ehartford/Wizard-Vicuna-7B-Uncensored")


pipe = pipeline(
    "text-generation",
    model=model,
    tokenizer=tokenizer,
    max_length=2048,
    temperature=0,
    top_p=0.95,
    repetition_penalty=1.15
)

local_llm = HuggingFacePipeline(pipeline=pipe)

def process_llm_response(llm_response):
    print(llm_response['result'])
    print('\n\nSources:')
    for source in llm_response["source_documents"]:
        print(source.metadata['source'])

embeddings = HuggingFaceInstructEmbeddings(model_name='hkunlp/instructor-xl')

pinecone.init(api_key='b4b7947c-96fd-4c95-9785-9c8ced03b64b', environment='us-west1-gcp-free')

index_name = "test"  # replace with your index name

pineconedb = Pinecone.from_existing_index(embedding=embeddings, index_name=index_name)

retriever = pineconedb.as_retriever(search_kwargs={"k": 1})

qa_chain = RetrievalQA.from_chain_type(llm=local_llm, chain_type="stuff", retriever=retriever, return_source_documents=True)

query = "tell me about Stilian Kutinchev "
llm_response = qa_chain(query)
process_llm_response(llm_response)