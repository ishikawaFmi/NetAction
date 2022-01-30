const Utils = require("./Utils");
const RoomList = require("./RoomList.js");
const Room = require("./Room.js");
var utilsIns = new Utils();
var roomList = new RoomList();
class Sort{
    constructor(server){
    
    this.Server = server;
}
sortMessage(msg,port,address){
    var json = JSON.parse(msg);
    console.log(json)
    switch(json["State"]){
        case 0://ログインメッセージの処理
        // if(roomList.length == 0){
        //     var room = new Room(2,"aaa");
        //     roomList.push(room);
        // }
        var room = new Room(2,"aaa");
        roomList.RoomList(room);
        var rooms = roomList.getRoomList;
        var data = {};
        data["State"] = "RoomList";
        data["RoomList"] = rooms;
       
        var js = JSON.stringify(data);
        utilsIns.sendJson(js,port,address,this.Server);
    }
    }
    
}
module.exports = Sort;

