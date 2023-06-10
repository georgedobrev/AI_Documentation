from transformers import T5Tokenizer, T5Model

class T5EmbeddingService:
    def __init__(self):
        self.tokenizer = T5Tokenizer.from_pretrained('t5-base')
        self.model = T5Model.from_pretrained('t5-base')

    def get_document_embedding(self, document_text):
        inputs = self.tokenizer(document_text, return_tensors="pt")
        outputs = self.model(**inputs)
        document_embedding = outputs.last_hidden_state.mean(dim=1).detach().numpy()
        return document_embedding