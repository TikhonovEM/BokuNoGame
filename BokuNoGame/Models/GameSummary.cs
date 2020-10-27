namespace BokuNoGame.Models
{
    public class GameSummary
    {
        public int Id { get; set; }
        public string GameName { get; set; }
        public int Rate { get; set; }
        public Genre Genre { get; set; }
        public State State { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public string UserId { get; set; }
    }
}