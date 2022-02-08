const Room = function (RoomID, MaxRoomMenber, RoomName) {
    this.RoomID = RoomID;
    this.MaxRoomMenber = MaxRoomMenber;
    this.RoomName = RoomName;
    this.RoomVisible = true;
    this.PlayerA = null;
    this.PlayerB = null;
}
Room.prototype.SendRoom = function (js, playerList, server, utils) {
    const players = this.Players(playerList);

    players.forEach(player => {
        utils.SendJson(js, player.PlayerPort, player.PlayerAddres, server);
    });


}
Room.prototype.SetColor = function (playerList, server, utils) {
    const players = this.Players(playerList);
    for (var i = 0; i <= 1; i++) {
        if (i == 0) {
            players[i].SetColor = 'Black';

            const data = {};
            data['State'] = 'SetColor';
            data['SetColor'] = players[i].SetColor;
            
            const js = JSON.stringify(data);

            utils.SendJson(js, players[i].PlayerPort, players[i].PlayerAddres, server);
        } else {
            players[i].SetColor = 'Red';

            const data = {};
            data['State'] = 'SetColor';
            data['SetColor'] = players[i].SetColor;

            const js = JSON.stringify(data);

            utils.SendJson(js, players[i].PlayerPort, players[i].PlayerAddres, server);
        }
    }
}
Room.prototype.Players = function (playerList) {
    players = [];

    playerA = playerList.GetPlayer(this.PlayerA);
    playerB = playerList.GetPlayer(this.PlayerB);

    players.push(playerA);
    players.push(playerB);

    return players;
}
module.exports = Room;