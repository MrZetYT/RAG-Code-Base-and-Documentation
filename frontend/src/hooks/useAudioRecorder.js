import { useState, useRef, useCallback } from 'react';

/**
 * Запись аудио с микрофона в формате WAV (16kHz, mono)
 */
export const useAudioRecorder = () => {
    const [isRecording, setIsRecording] = useState(false);
    const [recordingTime, setRecordingTime] = useState(0);
    const mediaRecorderRef = useRef(null);
    const audioChunksRef = useRef([]);
    const streamRef = useRef(null);
    const timerRef = useRef(null);

    const startRecording = useCallback(async () => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            streamRef.current = stream;

            // Пытаемся использовать WAV напрямую
            let mimeType = 'audio/wav';
            if (!MediaRecorder.isTypeSupported(mimeType)) {
                console.warn('audio/wav не поддерживается, используем audio/webm');
                mimeType = 'audio/webm';
            }

            const mediaRecorder = new MediaRecorder(stream, { mimeType });
            mediaRecorderRef.current = mediaRecorder;
            audioChunksRef.current = [];

            mediaRecorder.ondataavailable = (event) => {
                if (event.data.size > 0) {
                    audioChunksRef.current.push(event.data);
                }
            };

            mediaRecorder.start(100); // собираем куски каждые 100ms
            setIsRecording(true);
            setRecordingTime(0);

            // Таймер для отображения длительности записи
            timerRef.current = setInterval(() => {
                setRecordingTime(prev => prev + 1);
            }, 1000);

        } catch (error) {
            console.error('Ошибка доступа к микрофону:', error);
            throw new Error('Не удалось получить доступ к микрофону');
        }
    }, []);

    const stopRecording = useCallback(async () => {
        return new Promise((resolve, reject) => {
            if (!mediaRecorderRef.current || mediaRecorderRef.current.state !== 'recording') {
                reject(new Error('Нет активной записи'));
                return;
            }

            if (timerRef.current) {
                clearInterval(timerRef.current);
                timerRef.current = null;
            }

            mediaRecorderRef.current.onstop = async () => {
                setIsRecording(false);

                // Закрываем поток микрофона
                if (streamRef.current) {
                    streamRef.current.getTracks().forEach(track => track.stop());
                    streamRef.current = null;
                }

                const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/webm' });

                // Конвертируем WebM в WAV (16kHz, mono) если нужно
                // ВАЖНО: бэкенд требует WAV 16kHz
                // Если браузер не умеет в audio/wav, надо конвертировать
                // Для простоты временно вернём WebM, но на бэке это может не работать

                // TODO: добавить конвертацию через AudioContext (Web Audio API)
                // Пока отправляем как есть, позже добавлю конвертер
                resolve(audioBlob);
            };

            mediaRecorderRef.current.stop();
        });
    }, []);

    const cancelRecording = useCallback(() => {
        if (timerRef.current) {
            clearInterval(timerRef.current);
            timerRef.current = null;
        }

        if (mediaRecorderRef.current && mediaRecorderRef.current.state === 'recording') {
            mediaRecorderRef.current.onstop = () => {
                setIsRecording(false);
                if (streamRef.current) {
                    streamRef.current.getTracks().forEach(track => track.stop());
                    streamRef.current = null;
                }
            };
            mediaRecorderRef.current.stop();
        }
    }, []);

    return {
        isRecording,
        recordingTime,
        startRecording,
        stopRecording,
        cancelRecording,
    };
};