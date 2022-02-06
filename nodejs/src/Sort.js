const Sort = function (server,RoomList,PlayerList,Utils) {
    this.server = server;
    this.roomList = RoomList;
    this.playerList =PlayerList;
    this.utils = Utils;
}

Sort.prototype.SortMessage = function (msg, port, address) {

    var json = JSON.parse(msg);

    console.log(json)

    switch (json['State']) {
        case 0://ログインメッセージの処理
            this.playerList.CreatePlayer(port, address, this.server);//プレイヤーの作成
            this.roomList.SendRoomList(port, address, this.server);//ルームリストの送信

            break;
        case 1://ログアウト時の処理
            var player = this.playerList.GetPlayerList();

            player.forEach(x => {
                if (x.PlayerId == json['PlayerId']) player.splice(x);
            });

            break;
        case 2://メソッド同期
            break;

        case 3://サーバーのメソッドの実行
            switch (json['Name']) {
                case 'CreateRoom':
                    var createRoom = JSON.parse(json['Method']);

                    var room = this.roomList.CreateRoom(createRoom['RoomName'], port, address, this.Server);
                    var player = this.playerList.getPlayerList;

                    room.PlayerA = json['PlayerId'];

                    this.roomList.SendRoomList(port, address, this.server);
                    break;
                case 'EnterRoom':
                    var enterRoom = JSON.parse(json['Method']);
                    var room = this.roomList.GetRoom(enterRoom['RoomID']);

                    room.RoomVisible = false;
                    room.PlayerB = json['PlayerId'];
                    this.utils.GameStart(room,this.playerList,this.server);
                    console.log(room);
                    break;
            }
    }
}


module.exports = Sort;

