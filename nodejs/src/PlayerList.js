const Player = require('./Player');

const PlayerList = function (Utils) {
    this.playerList = [];
    this.PlayerIdList = [];
    this.utils = Utils;
}

PlayerList.prototype.GetPlayerList = function () {
    return this.playerList;
}

PlayerList.prototype.SetPlayerList = function (value) {
    this.playerList.push(value);
}

PlayerList.prototype.GetPlayer = function (value) {
    var returnPlayer;
    this.playerList.forEach(player => {
        if (player.PlayerId == value) {
            returnPlayer = player;
        }
    });
    return returnPlayer;
}

PlayerList.prototype.CreatePlayer = function (port, address,server) {
    var player = new Player(this.NewPlayerID(), port, address);
    this.SetPlayerList(player);

    var data = {};
    data['State'] = 'PlayerId';
    data['PlayerID'] = player.PlayerId;

    var json = JSON.stringify(data);
    this.utils.SendJson(json, port, address, server);
}

PlayerList.prototype.NewPlayerID = function () {
    var id = this.PlayerIdList.length + 1;
    this.PlayerIdList.push(id);
    
    return id;
}

module.eplayerports = PlayerList;