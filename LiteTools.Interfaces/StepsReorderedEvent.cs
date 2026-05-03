using System;
using System.Collections.Generic;
using System.Text;

namespace LiteTools.Interfaces
{
    // ========================================================================
    // 4. EVENTOS DE PASSOS E METADADOS (LITEFLOW -> ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Evento disparado quando a ordem dos passos é alterada (Drag & Drop).
    /// </summary>
    public class StepsReorderedEvent
    {
        public List<string> NewOrderIds { get; }
        public StepsReorderedEvent(List<string> newOrderIds) { NewOrderIds = newOrderIds; }
    }
}
