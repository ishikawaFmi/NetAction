const Utils = function () { };

Utils.prototype.SendJson = function (js, port, address, server) {
    console.log(new String(js))

    server.send(js, port, address);
}

Utils.prototype.GameStart = function () {

}
module.exports = Utils;