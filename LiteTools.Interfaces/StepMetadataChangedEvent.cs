namespace LiteTools.Interfaces
{
    // ========================================================================
    // 4. EVENTOS DE PASSOS E METADADOS (LITEFLOW -> ECOSSISTEMA)
    // ========================================================================

    /// <summary>
    /// Evento disparado pelo LiteFlow quando o utilizador altera os detalhes de um passo.
    /// </summary>
    public class StepMetadataChangedEvent
    {
        public string StepId { get; }
        public bool IsEvidenceOnly { get; }
        public string NewName { get; }

        public StepMetadataChangedEvent(string stepId, bool isEvidenceOnly, string newName)
        {
            StepId = stepId;
            IsEvidenceOnly = isEvidenceOnly;
            NewName = newName;
        }
    }
}