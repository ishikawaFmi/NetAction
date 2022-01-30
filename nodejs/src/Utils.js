class Utils{
sendJson(js,port,address,server){
console.log(new String(js))
server.send(js,port,address);
}
}
module.exports = Utils;
