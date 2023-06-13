from langchain.llms import HuggingFacePipeline
import torch
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer, AutoModelForCausalLM, pipeline
from langchain.embeddings import HuggingFaceEmbeddings

model_id = 'google/flan-t5-base'
tokenizer = AutoTokenizer.from_pretrained(model_id)
model = AutoModelForSeq2SeqLM.from_pretrained(model_id)

pipe = pipeline(
    "text2text-generation",
    model=model,
    tokenizer=tokenizer
)

local_llm = HuggingFacePipeline(pipeline=pipe)

print(local_llm('translate English to German:How old are you'))

class T5Embeddings(HuggingFaceEmbeddings):
    def __init__(self, model_id):
        self.tokenizer = AutoTokenizer.from_pretrained(model_id)
        self.model = AutoModelForSeq2SeqLM.from_pretrained(model_id)

    async def transform_documents(self, documents):
        results = []
        for document in documents:
            inputs = self.tokenizer.encode_plus(
                document.page_content,
                return_tensors="pt",
                max_length=512,
                truncation=True,
            )
            with torch.no_grad():
                outputs = self.model(**inputs)
            # Use the mean of the last hidden state to represent the document
            embedding = outputs.last_hidden_state.mean(dim=1).detach().numpy()
            results.append((document.metadata['start_index'], embedding))
        return results
