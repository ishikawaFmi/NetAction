const Player = function (PlayerID, PlayerPort, PlayerAddres) {
    this.PlayerId = PlayerID;
    this.PlayerPort = PlayerPort;
    this.PlayerAddres = PlayerAddres;
    this.PlayerColor = "None";
    this.CurrentRoom = null;
}
module.exports = Player;
