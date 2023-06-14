import React, { useState, ChangeEvent, FormEvent } from 'react';
import "../Styles/MainPageStyles.css";

interface Message {
  id: number;
  user: string;
  text: string;
}

function MainPage() {
  const [messages, setMessages] = useState<Message[]>([
    { id: 1, user: 'GPT', text: 'Hello, how can I assist you today?' }
  ]);

  const [input, setInput] = useState('');

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    setInput(e.target.value);
  }

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    // todo
    setInput('');
  }

  return (
    <div className="App">
      <div className="main-page">
        <div className="chat-window">
          {messages.map(message => (
            <div key={message.id} className={`message ${message.user}`}>
              <p>{message.text}</p>
            </div>
          ))}
        </div>
        <form onSubmit={handleSubmit} className="message-input-form">
          <input
            type="text"
            value={input}
            onChange={handleInputChange}
            className="message-input"
            placeholder="Type your message here..."
          />
          <button type="submit" className="submit-button">Send</button>
        </form>
      </div>
    </div>
  );
}

export default MainPage;
