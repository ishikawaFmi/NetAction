
const PORT = 50000;
const HOST = 'localHost';

const dgram = require('dgram');
const PlayerList = require('./src/PlayerList.js');
const RoomList = require('./src/RoomList.js');
var Sort = require('./src/Sort.js');
const Utils = require('./src/Utils.js');

const server = dgram.createSocket('udp4');
const utils = new Utils();
const playerList = new PlayerList(utils);
const roomList = new RoomList(utils);
var sort = new Sort(server,roomList,playerList,utils);

//エラー時の処理
server.on('error', (err) => {
    console.log(`sever error\n${err, stack}`);
    server.close;
});

//メッセージが届いたときの処理
server.on('message', (msg, rinfo) => {
    console.log(new String(msg));
    sort.SortMessage(msg, rinfo.port, rinfo.address);
});

//サーバーの受付が開始されたときの処理
server.on('listening', () => {
    const address = server.address();
    console.log(`server listerning ${address.address}:${address.port}`);
});

server.bind(PORT);