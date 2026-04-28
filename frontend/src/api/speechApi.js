import api from './client';

export const speechApi = {
    /**
     * Отправляет WAV файл на распознавание речи
     * @param {Blob} wavBlob - аудио в формате WAV (16kHz, mono)
     * @returns {Promise<{transcript: string}>}
     */
    recognizeSpeech: async (wavBlob) => {
        const formData = new FormData();
        formData.append('audio', wavBlob, 'recording.wav');

        // Важно: для FormData нельзя использовать api.interceptor (axios сам установит headers)
        const response = await fetch('/api/speech/recognize', {
            method: 'POST',
            body: formData,
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Ошибка распознавания речи');
        }

        const data = await response.json();
        return data; // { transcript: "..." }
    },
};