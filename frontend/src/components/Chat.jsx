import React, { useState, useRef, useEffect } from 'react';
import { chatApi } from '../api/chatApi';
import { mockChatAnswers } from '../utils/mockData';
import './Chat.css';

const Chat = () => {
    const [messages, setMessages] = useState([]);
    const [input, setInput] = useState('');
    const [loading, setLoading] = useState(false);
    const [useMock, setUseMock] = useState(true);
    const [isRecording, setIsRecording] = useState(false);
    const messagesEndRef = useRef(null);
    const recognitionRef = useRef(null);
    const [recordingTime, setRecordingTime] = useState(0);
    const timerRef = useRef(null);

    // Проверка поддержки SpeechRecognition
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    const isSpeechSupported = !!SpeechRecognition;

    // Инициализация распознавания речи
    useEffect(() => {
        if (isSpeechSupported) {
            recognitionRef.current = new SpeechRecognition();
            recognitionRef.current.lang = 'ru-RU';
            recognitionRef.current.interimResults = true;
            recognitionRef.current.continuous = true;
            recognitionRef.current.maxAlternatives = 1;

            recognitionRef.current.onresult = (event) => {
                const transcript = Array.from(event.results)
                    .map(result => result[0].transcript)
                    .join('');
                setInput(transcript);

                if (event.results[0].isFinal && !isRecording) {
                    handleSend(transcript);
                }
            };

            recognitionRef.current.onerror = (event) => {
                console.error('Speech recognition error:', event.error);
                setIsRecording(false);
                setRecordingTime(0);
                if (timerRef.current) clearInterval(timerRef.current);
            };

            recognitionRef.current.onend = () => {
                setIsRecording(false);
                setRecordingTime(0);
                if (timerRef.current) clearInterval(timerRef.current);
            };
        }
    }, []);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    // Таймер записи
    useEffect(() => {
        if (isRecording) {
            timerRef.current = setInterval(() => {
                setRecordingTime(prev => prev + 1);
            }, 1000);
        } else {
            if (timerRef.current) clearInterval(timerRef.current);
        }
        return () => {
            if (timerRef.current) clearInterval(timerRef.current);
        };
    }, [isRecording]);

    const startRecording = () => {
        if (!isSpeechSupported) {
            alert('Ваш браузер не поддерживает голосовой ввод. Используйте Chrome, Edge или Safari.');
            return;
        }

        try {
            recognitionRef.current.start();
            setIsRecording(true);
            setRecordingTime(0);
        } catch (err) {
            console.error('Failed to start recording:', err);
        }
    };

    const stopRecording = () => {
        if (recognitionRef.current && isRecording) {
            recognitionRef.current.stop();
        }
    };

    const getMockAnswer = (question) => {
        const lowerQuestion = question.toLowerCase();

        if (lowerQuestion.includes('savefiles') || lowerQuestion.includes('сохраняет')) {
            return "📁 **Метод SaveFiles** сохраняет загруженные файлы на диск в папку Data, создает запись в базе данных PostgreSQL и запускает фоновую обработку через Hangfire. Каждый файл проходит парсинг и векторизацию. После успешной обработки статус файла меняется на 'Ready'.";
        }

        if (lowerQuestion.includes('векторизац') || lowerQuestion.includes('vectorization')) {
            return "🧠 **Векторизация** — это процесс преобразования кода в числовые векторы (эмбеддинги).\n\n" +
                "🔹 Используется модель **bge-m3** (размер эмбеддинга: 1024)\n" +
                "🔹 Каждый блок кода превращается в вектор\n" +
                "🔹 Векторы сохраняются в **Qdrant** (векторная БД)\n" +
                "🔹 Это позволяет находить семантически похожие блоки кода\n\n" +
                "Процесс автоматический и запускается после загрузки файлов.";
        }

        if (lowerQuestion.includes('поиск') || lowerQuestion.includes('search')) {
            return "🔍 **Как работает поиск:**\n\n" +
                "1️⃣ Ваш вопрос преобразуется в вектор (эмбеддинг)\n" +
                "2️⃣ Система ищет похожие векторы в Qdrant\n" +
                "3️⃣ Возвращаются топ-5 наиболее похожих блоков кода\n" +
                "4️⃣ Каждый результат имеет процент схожести (чем выше, тем ближе по смыслу)\n\n" +
                "Поиск работает на семантическом уровне, поэтому можно искать не по точным словам, а по смыслу!";
        }

        if (lowerQuestion.includes('база') || lowerQuestion.includes('database') || lowerQuestion.includes('бд')) {
            return "🗄️ **База данных:**\n\n" +
                "🔹 **PostgreSQL** — основная БД\n" +
                "🔹 **Entity Framework Core** — ORM для работы с БД\n" +
                "🔹 **Таблицы:**\n" +
                "   - `FileItems` — метаданные загруженных файлов\n" +
                "   - `InfoBlocks` — блоки кода (методы, классы и т.д.)\n" +
                "🔹 Строка подключения настраивается в `appsettings.json`\n" +
                "🔹 Миграции применяются через `dotnet ef database update`";
        }

        if (lowerQuestion.includes('парсинг') || lowerQuestion.includes('parser')) {
            return "📖 **Парсинг файлов:**\n\n" +
                "Система поддерживает множество языков программирования:\n" +
                "• **C#** — через Roslyn\n" +
                "• **Python, JavaScript, TypeScript, Java, C++, Rust, PHP** — через Tree-sitter\n" +
                "• **Markdown, PDF, DOCX, TXT** — через специализированные парсеры\n\n" +
                "Каждый файл разбивается на логические блоки: классы, методы, интерфейсы, параграфы и т.д.";
        }

        if (lowerQuestion.includes('hangfire')) {
            return "⏰ **Hangfire** — это фреймворк для фоновой обработки.\n\n" +
                "Он используется для:\n" +
                "• Асинхронного парсинга файлов после загрузки\n" +
                "• Векторизации блоков кода в фоне\n" +
                "• Отслеживания статуса обработки\n\n" +
                "Дашборд Hangfire доступен по адресу: `/hangfire`";
        }

        if (lowerQuestion.includes('qdrant')) {
            return "⚡ **Qdrant** — это векторная база данных.\n\n" +
                "🔹 Хранит эмбеддинги блоков кода\n" +
                "🔹 Размер вектора: 1024\n" +
                "🔹 Использует косинусное расстояние для поиска\n" +
                "🔹 Позволяет находить семантически похожие блоки\n\n" +
                "Настройки Qdrant находятся в `appsettings.json` (хост и порт)";
        }

        return "🤖 **Я AI Ассистент GZ-Coder!**\n\n" +
            "Я могу ответить на вопросы о проекте:\n" +
            "• 📁 **Загрузка файлов** — как сохраняются и обрабатываются файлы\n" +
            "• 🧠 **Векторизация** — как код превращается в эмбеддинги\n" +
            "• 🔍 **Поиск** — как работает семантический поиск\n" +
            "• 🗄️ **База данных** — структура PostgreSQL\n" +
            "• 📖 **Парсеры** — поддержка языков программирования\n" +
            "• ⚡ **Hangfire** — фоновая обработка\n" +
            "• 🎯 **Qdrant** — векторное хранилище\n\n" +
            "Задайте конкретный вопрос, и я подробно расскажу!";
    };

    const handleSend = async (textToSend = null) => {
        const question = textToSend !== null ? textToSend : input;
        if (!question.trim()) return;

        const userMessage = {
            id: Date.now(),
            type: 'user',
            content: question,
            timestamp: new Date(),
        };

        setMessages(prev => [...prev, userMessage]);
        setInput('');
        setLoading(true);

        try {
            let answer;
            if (useMock) {
                await new Promise(resolve => setTimeout(resolve, 1000));
                answer = getMockAnswer(question);
            } else {
                const response = await chatApi.askQuestion(question);
                answer = response.answer;
            }

            const assistantMessage = {
                id: Date.now() + 1,
                type: 'assistant',
                content: answer,
                timestamp: new Date(),
            };
            setMessages(prev => [...prev, assistantMessage]);
        } catch (error) {
            const errorMessage = {
                id: Date.now() + 1,
                type: 'assistant',
                content: `❌ **Ошибка:** ${error.message}\n\nПроверьте, запущен ли бэкенд, или включите Демо-режим.`,
                isError: true,
                timestamp: new Date(),
            };
            setMessages(prev => [...prev, errorMessage]);
        } finally {
            setLoading(false);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    const formatTime = (date) => {
        return new Date(date).toLocaleTimeString('ru-RU', {
            hour: '2-digit',
            minute: '2-digit',
        });
    };

    const formatRecordingTime = (seconds) => {
        const mins = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    };

    // Функция для форматирования текста с маркдауном
    const formatMessage = (text) => {
        if (!text) return text;

        // Заменяем **текст** на <strong>текст</strong>
        let formatted = text.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');

        // Заменяем переносы строк на <br/>
        formatted = formatted.replace(/\n/g, '<br/>');

        // Заменяем маркированные списки
        formatted = formatted.replace(/• /g, '<span class="bullet">•</span> ');

        return { __html: formatted };
    };

    return (
        <div className="chat-container">
            <div className="chat-header">
               
                <h2>🤖 AI Ассистент</h2>
                <p>Задайте вопрос о загруженном проекте</p>
                <div className="chat-controls">
                    <label className="mock-toggle">
                        <input type="checkbox" checked={useMock} onChange={(e) => setUseMock(e.target.checked)} />
                        <span>Демо-режим</span>
                    </label>
                    {isSpeechSupported && (
                        <span className="voice-badge">
                            <span className="voice-icon">🎤</span> Голосовой ввод
                        </span>
                    )}
                </div>
            </div>

            <div className="messages-container">
                {messages.length === 0 && (
                    <div className="welcome-message">
                        <div className="welcome-icon">🤖</div>
                        <h3>AI Ассистент готов к работе</h3>
                        <p>Я проанализировал ваш проект и готов ответить на вопросы о коде</p>
                        <p className="welcome-sub">💡 Спроси меня:</p>
                        <div className="suggestions">
                            <button onClick={() => setInput("Что делает метод SaveFiles?")}>
                                📁 Что делает SaveFiles?
                            </button>
                            <button onClick={() => setInput("Как работает векторизация?")}>
                                🧠 Как работает векторизация?
                            </button>
                            <button onClick={() => setInput("Как работает поиск по коду?")}>
                                🔍 Как работает поиск?
                            </button>
                            <button onClick={() => setInput("Какая база данных используется?")}>
                                🗄️ Какая БД используется?
                            </button>
                            <button onClick={() => setInput("Как работает парсинг файлов?")}>
                                📖 Как работает парсинг?
                            </button>
                            <button onClick={() => setInput("Что такое Qdrant?")}>
                                ⚡ Что такое Qdrant?
                            </button>
                        </div>
                        {isSpeechSupported && (
                            <p className="voice-tip">
                                <span className="mic-icon">🎤</span> Нажмите и удерживайте кнопку микрофона, чтобы спросить голосом
                            </p>
                        )}
                    </div>
                )}

                {messages.map((msg) => (
                    <div key={msg.id} className={`message ${msg.type}`}>
                        <div className="message-header">
                            <span className="message-author">
                                {msg.type === 'user' ? '👤 Вы' : '🤖 AI Ассистент'}
                            </span>
                            <span className="message-time">{formatTime(msg.timestamp)}</span>
                        </div>
                        <div className={`message-content ${msg.isError ? 'error' : ''}`}>
                            {msg.type === 'assistant' && !msg.isError ? (
                                <div dangerouslySetInnerHTML={formatMessage(msg.content)} />
                            ) : (
                                msg.content
                            )}
                        </div>
                    </div>
                ))}

                {loading && (
                    <div className="message assistant loading">
                        <div className="message-content">
                            <div className="typing-indicator">
                                <span></span>
                                <span></span>
                                <span></span>
                            </div>
                            <span> AI анализирует ваш вопрос...</span>
                        </div>
                    </div>
                )}

                <div ref={messagesEndRef} />
            </div>

            <div className="input-area">
                <div className="input-wrapper">
                    <textarea
                        value={input}
                        onChange={(e) => setInput(e.target.value)}
                        onKeyPress={handleKeyPress}
                        placeholder="Спросите о проекте..."
                        rows={3}
                        disabled={loading}
                    />
                    {isSpeechSupported && (
                        <button
                            className={`voice-btn ${isRecording ? 'recording' : ''}`}
                            onMouseDown={startRecording}
                            onMouseUp={stopRecording}
                            onMouseLeave={stopRecording}
                            onTouchStart={startRecording}
                            onTouchEnd={stopRecording}
                            disabled={loading}
                            title={isRecording ? 'Отпустите для отправки' : 'Нажмите и говорите'}
                        >
                            {isRecording ? (
                                <>
                                    <span className="recording-icon">🔴</span>
                                    <span className="recording-time">{formatRecordingTime(recordingTime)}</span>
                                </>
                            ) : (
                                '🎤'
                            )}
                        </button>
                    )}
                </div>
                <button
                    onClick={() => handleSend()}
                    disabled={loading || !input.trim()}
                    className="send-btn"
                >
                    ✈️
                </button>
            </div>
        </div>
    );
};

export default Chat;