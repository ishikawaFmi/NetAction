const Utils = function () { };

Utils.prototype.SendJson = function (js, port, address, server) {
    console.log(new String(js))

    server.send(js, port, address);
}
module.exports = Utils;