namespace producer.Models
{
    public class Card112ChangedRequest
    {
        public string GlobalId { get; set; }
        public string EmergencyCardId { get; set; }
        public DateTime DtCreate { get; set; }
        public string CallTypeId { get; set; }
        public string CardSyntheticState { get; set; }
        public string Card01SyntheticState { get; set; }
        public string Card02SyntheticState { get; set; }
        public string CardCommServSyntheticState { get; set; }
        public string CardATSyntheticState { get; set; }
        public string WithCall { get; set; }
        public string Creator { get; set; }
        public string IncidentTypeID { get; set; }
        public string IncidentType { get; set; }
        public string Near { get; set; }
        public string HumanThreat { get; set; }
    }
}
