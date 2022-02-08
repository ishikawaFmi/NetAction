const Sort = function (server, RoomList, PlayerList, Utils) {
    this.server = server;
    this.roomList = RoomList;
    this.playerList = PlayerList;
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
                    var player = this.playerList.GetPlayer(json['PlayerId']);

                    console.log(json['PlayerId']);

                    room.PlayerA = json['PlayerId'];
                    player.CurrentRoom = room.RoomID;//プレイヤーの現状のルームを更新する           

                    this.roomList.SendRoomList(port, address, this.server);
                    break;
                case 'EnterRoom':
                    var enterRoom = JSON.parse(json['Method']);
                    var room = this.roomList.GetRoom(enterRoom['RoomID']);
                    var player = this.playerList.GetPlayer(json['PlayerId']);

                    room.RoomVisible = false;
                    room.PlayerB = json['PlayerId'];
                    player.CurrentRoom = room.RoomID;//プレイヤーの現状のルームを更新する

                    room.SetColor(this.playerList,this.server, this.utils);

                    var data = {};
                    data['State'] = 'GameSceneLoad';

                    const js = JSON.stringify(data);

                    room.SendRoom(js, this.playerList, this.server, this.utils);
                    console.log(room);
                    break;
                case 'GameScene':
                    var roomId = this.playerList.GetPlayer(json['PlayerId']).CurrentRoom;

            }
    }
}


module.exports = Sort;

