const Room = function (RoomID, MaxRoomMenber, RoomName) {
    this.RoomID = RoomID;
    this.MaxRoomMenber = MaxRoomMenber;
    this.RoomName = RoomName;
    this.RoomVisible = true;
    this.PlayerA;
    this.PlayerB;
    this.RoomPreparation = 0;
}
Room.prototype.SendRoom = function (js, playerList, server, utils) {
    const players = this.Players(playerList);

    players.forEach(player => {
        utils.SendJson(js, player.PlayerPort, player.PlayerAddres, server);
    });


}

Room.prototype.Delete = function (playerList, server, utils) {
    const players = this.Players(playerList);
    players.forEach(player => {
        player.PlayerColor = "None";
        player.CurrentRoom = null;
        player.IsNotCard = false;
    });
    var data = {};

    data['State'] = 'RoomDelete';
    const js = JSON.stringify(data);
    this.SendRoom(js, playerList, server, utils);
}

Room.prototype.SetColor = function (playerList, server, utils) {
    const players = this.Players(playerList);
    for (var i = 0; i <= 1; i++) {
        if (i == 0) {
            players[i].PlayerColor = 'Black';

            const data = {};
            data['State'] = 'SetColor';
            data['SetColor'] = players[i].PlayerColor;

            const js = JSON.stringify(data);

            utils.SendJson(js, players[i].PlayerPort, players[i].PlayerAddres, server);
        } else {
            players[i].PlayerColor = 'Red';

            const data = {};
            data['State'] = 'SetColor';
            data['SetColor'] = players[i].PlayerColor;

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

Room.prototype.NotCard = function (playerList, server, utils) {
    const players = this.Players(playerList);

    if (players[0].IsNotCard == true && players[1].IsNotCard == true) {

        players[0].IsNotCard = false;
        players[1].IsNotCard = false;

        var data = {};

        data['State'] = 'NotCard';

        var js = JSON.stringify(data);
        this.SendRoom(js, playerList, server, utils);
    }
}

Room.prototype.ResetNotCard = function (playerList) {
    const players = this.Players(playerList);

    if (players[0].IsNotCard == true && players[1].IsNotCard == true) {

        players[0].IsNotCard = false;
        players[1].IsNotCard = false;
    }
}
module.exports = Room;