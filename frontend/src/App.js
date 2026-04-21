import React, { useState } from 'react';
import FileUpload from './components/FileUpload';
import Chat from './components/Chat';
import './App.css';

function App() {
    const [activeTab, setActiveTab] = useState('upload'); // 'upload' или 'chat'

    return (
        <div className="App">
            <div className="app-container">
                <header className="app-header">
                    <h1>GZ-Coder</h1>
                    <p>RAG Code Assistant</p>
                </header>

                {/* Навигация */}
                <nav className="app-nav">
                    <button
                        className={`nav-btn ${activeTab === 'upload' ? 'active' : ''}`}
                        onClick={() => setActiveTab('upload')}
                    >
                        📁 Загрузка проекта
                    </button>
                    <button
                        className={`nav-btn ${activeTab === 'chat' ? 'active' : ''}`}
                        onClick={() => setActiveTab('chat')}
                    >
                        🤖 AI Ассистент
                    </button>
                </nav>

                {/* Контент */}
                <main className="app-main">
                    {activeTab === 'upload' ? <FileUpload /> : <Chat />}
                </main>

                <footer className="app-footer">
                    <p>GZ-Coder v2.0</p>
                </footer>
            </div>
        </div>
    );
}

export default App;