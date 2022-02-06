const Room = function (RoomID, MaxRoomMenber, RoomName, RoomVisible) {
    this.RoomID = RoomID;
    this.MaxRoomMenber = MaxRoomMenber;
    this.RoomName = RoomName;
    this.RoomVisible = true;
    this.PlayerA = null;
    this.PlayerB = null;
}
module.exports = Room;