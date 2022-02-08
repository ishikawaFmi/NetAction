const Room = require('./Room');

const RoomList = function (Utils) {
    this.roomList = [];
    this.roomIdList = [];
    this.utils = Utils;
}

// ルームリストを返す
RoomList.prototype.GetRoomList = function () {
    return this.roomList;
}

// ルームリストにルームを追加する
RoomList.prototype.SetRoomList = function (value) {
    this.roomList.push(value);
}

// ルームIDに応じたルームを返す
RoomList.prototype.GetRoom = function (values) {
    var room;
    this.roomList.forEach(x => {
        if (x.RoomID == values) {
            room = x;
        }
    })
    return room;
}

// ルームを作成する
RoomList.prototype.CreateRoom = function (roomName) {
    const room = new Room(this.NewRoomID(), 2, roomName);
    this.SetRoomList(room);
    return room;
}

RoomList.prototype.SendRoomList = function (port, address, server) {
    const currentRoomList = [];

    this.roomList.forEach(room => {
        if (room.RoomVisible) currentRoomList.push(room); // RoomVisibleがtrueの場合のみ追加する
    })

    const data = {};
    data['State'] = 'RoomList';
    data['RoomList'] = currentRoomList;
    if (currentRoomList.length >= 1) {
        const js = JSON.stringify(data);
        this.utils.SendJson(js, port, address, server);
    }
}

RoomList.prototype.NewRoomID = function () {
    const id = this.roomIdList.length + 1;

    this.roomIdList.push(id);
    return id;
}

module.exports = RoomList;