using System;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 1. INTERFACES PRINCIPAIS (O DNA DO ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Contrato do Barramento de Eventos (Padrão Mediator/PubSub).
    /// Permite que plugins conversem entre si sem se conhecerem diretamente.
    /// </summary>
    public interface IEventBus
    {
        // Assina um evento genérico
        void Subscribe<TEvent>(Action<TEvent> handler);

        // Publica um evento genérico para todos os assinantes
        void Publish<TEvent>(TEvent eventItem);
    }
}