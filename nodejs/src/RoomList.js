class RoomList{
roomList = new Array();

get getRoomList(){
    return this.roomList;
}

RoomList(value){
    this.roomList.push(value);
}

}
module.exports = RoomList;


