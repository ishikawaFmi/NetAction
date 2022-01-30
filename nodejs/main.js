
const PORT = 50000;
const HOST = "localHost";
const dgram = require('dgram');
const server = dgram.createSocket('udp4');
var Sort = require('./src/Sort.js');
var SortIns = new Sort(server);

//エラー時の処理
server.on('error',(err)=>{
    console.log(`sever error\n${err,stack}`);
    server.close;
    
});

//メッセージが届いたときの処理
server.on('message',(msg,rinfo)=>{
console.log(new String(msg));
SortIns.sortMessage(msg,rinfo.port,rinfo.address);
});

//サーバーの受付が開始されたときの処理
server.on('listening',()=>{
    const address = server.address();
    console.log(`server listerning ${address.address}:${address.port}`);
   
});

server.bind(PORT);