using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMKit.Model;
using LMKit.TextGeneration;
using Microsoft.Extensions.Logging;

namespace RAG_Code_Base.Services.Explanation
{
    public class ExplanationService
    {
        private readonly LM _model;
        private readonly MultiTurnConversation _conversation;
        private readonly ILogger<ExplanationService>? _logger;

        public ExplanationService(ILogger<ExplanationService>? logger = null)
        {
            _logger = logger;

            try
            {
                var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "LM", "gemma-3-it-1B-Q4_K_M.gguf");
                _model = new LM(modelPath);
                _conversation = new MultiTurnConversation(_model);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при инициализации модели LLM.");
                throw;
            }
        }

        /// <summary>
        /// Объясняет фрагмент кода или текст документации.
        /// Контекст безопасно обрабатывается даже если пустой.
        /// </summary>
        public async Task<string> ExplainAsync(string question, List<string>? contexts = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Формируем контекст, убираем пустые строки
                string contextBlock = contexts != null
                    ? string.Join("\n\n", contexts.Where(c => !string.IsNullOrWhiteSpace(c)))
                    : "";

                if (string.IsNullOrWhiteSpace(contextBlock))
                    contextBlock = "Контекст отсутствует.";

                string prompt = $@"
Ты — инженер-программист. Объясни код и технический текст простыми словами.
Если данных недостаточно — честно скажи, что данных не хватает.

Контекст:
{contextBlock}

Вопрос:
{question}

Ответ:
";

                // Генерация ответа
                var response = await _conversation.SubmitAsync(prompt, cancellationToken);

                // Проверка на null и пустой Completion
                if (response == null || string.IsNullOrWhiteSpace(response.Completion))
                {
                    _logger?.LogWarning("Модель вернула пустой ответ или null.");
                    return "Модель не смогла сгенерировать ответ.";
                }

                return response.Completion.Trim();
            }
            catch (OperationCanceledException)
            {
                _logger?.LogWarning("Генерация ответа была отменена пользователем.");
                return "Генерация отменена.";
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при генерации объяснения.");
                return "Произошла ошибка при объяснении.";
            }
        }
    }
}
