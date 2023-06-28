import os
import pandas as pd
from transformers import AutoModelForSeq2SeqLM, AutoTokenizer, DataCollatorForSeq2Seq, Seq2SeqTrainer, Seq2SeqTrainingArguments
from datasets import Dataset

df = pd.read_csv("./news_summary.csv", encoding='latin-1', usecols=['headlines', 'text'])

df = df.rename(columns={"headlines":"target_text", "text":"source_text"})
df = df[['source_text', 'target_text']]

print(f"Train dataset size: {len(df['source_text'])}")


base_model = "google/flan-t5-base"
save_dir = "./saved_model"
model_path = save_dir if os.path.exists(save_dir) else base_model
model = AutoModelForSeq2SeqLM.from_pretrained(model_path)
tokenizer = AutoTokenizer.from_pretrained(base_model)


dataset = Dataset.from_pandas(df)

def preprocess_function(examples):
    inputs = examples["source_text"]
    targets = examples["target_text"]
    model_inputs = tokenizer(inputs, max_length=512, padding="max_length", truncation=True)

    # Setup the tokenizer for targets
    with tokenizer.as_target_tokenizer():
        labels = tokenizer(targets, max_length=512, truncation=True)

    model_inputs["labels"] = labels["input_ids"]
    return model_inputs

train_dataset = dataset.map(preprocess_function, batched=True)

# Step 4: Define the Training Arguments
training_args = Seq2SeqTrainingArguments(
    output_dir="./output",
    num_train_epochs=5,
    per_device_train_batch_size=8,
    learning_rate=1e-4,
    save_strategy="epoch",
)

data_collator = DataCollatorForSeq2Seq(tokenizer, model)

# Step 5: Create the Trainer
trainer = Seq2SeqTrainer(
    model,
    training_args,
    train_dataset=train_dataset,
    data_collator=data_collator,
    tokenizer=tokenizer
)

# Step 6: Start Training
trainer.train()

os.makedirs(save_dir, exist_ok=True)

model.save_pretrained(save_dir)
tokenizer.save_pretrained(save_dir)