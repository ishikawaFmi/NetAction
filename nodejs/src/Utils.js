const Utils = function () { };

Utils.prototype.SendJson = function (js, port, address, server) {
    console.log(new String(js))
    server.send(js, port, address);
}

Utils.prototype.GameStart = function (room, playerList, server) {
    players = [];

    players.push(playerList.GetPlayer(room.PlayerA));
    players.push(playerList.GetPlayer(room.PlayerB));

    players.forEach(player => {
        var data = {};
        data['State'] = 'GameStart';
        const js = JSON.stringify(data);
        this.SendJson(js, player.PlayerPort, player.PlayerAddres, server);
    });
}
module.exports = Utils;