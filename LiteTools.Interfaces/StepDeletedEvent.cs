using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 4. EVENTOS DE PASSOS E METADADOS (LITEFLOW -> ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Evento disparado quando um passo é apagado pelo utilizador.
    /// </summary>
    public class StepDeletedEvent
    {
        public string StepId { get; }
        public StepDeletedEvent(string stepId) { StepId = stepId; }
    }
}
