using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 4. EVENTOS DE PASSOS E METADADOS (LITEFLOW -> ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Evento disparado quando um passo é restaurado (CTRL+Z ou Lixeira).
    /// </summary>
    public class StepRestoredEvent
    {
        public string StepId { get; }
        public StepRestoredEvent(string stepId) { StepId = stepId; }
    }
}
