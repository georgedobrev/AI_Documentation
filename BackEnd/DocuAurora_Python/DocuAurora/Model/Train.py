import os
import pandas as pd
from transformers import T5ForConditionalGeneration, AutoTokenizer, DataCollatorForSeq2Seq, Seq2SeqTrainer, Seq2SeqTrainingArguments
from datasets import Dataset
from transformers import pipeline

# Load your dataset
dataset = pd.read_csv("./news_summary.csv", encoding='latin-1', usecols=['headlines', 'text'])
dataset = dataset.rename(columns={"headlines":"target_text", "text":"source_text"})

# Model names and paths
qmodel_name = "ThomasSimonini/t5-end2end-question-generation"
amodel_name = "deepset/roberta-base-squad2"
base_model = "google/flan-t5-base"
save_dir = "./saved_model"

# Load models and tokenizers
qmodel = T5ForConditionalGeneration.from_pretrained(qmodel_name)
qtokenizer = AutoTokenizer.from_pretrained(base_model)
amodel = pipeline('question-answering', model=amodel_name, tokenizer=amodel_name)
model_path = save_dir if os.path.exists(save_dir) else base_model
model = T5ForConditionalGeneration.from_pretrained(model_path)
tokenizer = AutoTokenizer.from_pretrained(base_model)

# Question generation function
def run_qmodel(input_string, **generator_args):
    generator_args = {
        "max_length": 256,
        "num_beams": 4,
        "length_penalty": 1.5,
        "no_repeat_ngram_size": 3,
        "early_stopping": True,
    }
    input_string = "generate questions: " + input_string + " </s>"
    input_ids = qtokenizer.encode(input_string, return_tensors="pt")
    res = qmodel.generate(input_ids, **generator_args)
    output = qtokenizer.batch_decode(res, skip_special_tokens=True)
    output = [item.split("? ") for item in output]
    output = output[0]
    return output

qa_pairs = []
for index, row in dataset.iterrows():
    source_text = row["source_text"]
    print(f"Generating questions for text {index+1}/{len(dataset)}...")
    questions = run_qmodel(source_text)
    for question in questions:
        print(f"Answering question: {question}")
        # Generate an answer based on the question and the source text
        answer = amodel(question=question, context=source_text)
        qa_pairs.append({"question": question, "answer": answer["answer"]})

# Prepare the new dataframe for training
df = pd.DataFrame(qa_pairs)
print(f"Finished generating questions and answers. Preparing for training...")

dataset = Dataset.from_pandas(df)

# Preprocess function
def preprocess_function(examples):
    inputs = examples["question"]
    targets = examples["answer"]
    model_inputs = tokenizer(inputs, padding="max_length", truncation=True)

    with tokenizer.as_target_tokenizer():
        labels = tokenizer(targets, max_length=512, truncation=True)

    model_inputs["labels"] = labels["input_ids"]
    return model_inputs

print("Mapping preprocess function to the dataset...")
# Map the preprocess function to the dataset
train_dataset = dataset.map(preprocess_function, batched=True)

# Training arguments
training_args = Seq2SeqTrainingArguments(
    output_dir="./output",
    num_train_epochs=5,
    per_device_train_batch_size=8,
    learning_rate=1e-4,
    save_strategy="epoch",
)

# Data collator
data_collator = DataCollatorForSeq2Seq(tokenizer, model)

print("Setting up trainer...")
# Trainer
trainer = Seq2SeqTrainer(
    model,
    training_args,
    train_dataset=train_dataset,
    data_collator=data_collator,
    tokenizer=tokenizer
)

print("Starting training...")
# Start training
trainer.train()

print("Training finished. Saving the model...")
# Save the model
os.makedirs(save_dir, exist_ok=True)
model.save_pretrained(save_dir)
tokenizer.save_pretrained(save_dir)
print("Model saved.")