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
            var playerId = JSON.parse(json['PlayerId']);
            this.playerList.CreatePlayer(playerId['PlayerID'], port, address, this.server);//プレイヤーの作成
            this.roomList.SendRoomList(port, address, this.server);//ルームリストの送信
            break;
        case 1://ログアウト時の処理
            var player = this.playerList.GetPlayerList();
            var playerId = JSON.parse(json['PlayerId']);
            var roomList = this.roomList.GetRoomList();
            player.forEach(x => {
                if (x.PlayerId == playerId['PlayerID']) {
                    if (x.CurrentRoom != null) {
                        var room = this.roomList.GetRoom(x.CurrentRoom);
                        room.Delete(this.playerList, this.server, this.utils);
                        roomList.splice(room);
                    }
                    player.splice(x);
                }
            });
            break;
        case 2://メソッド同期
            switch (json['Name']) {
                case 'IncetanceCard':
                    var card = JSON.parse(json['Method']);
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);
                    var room = this.roomList.GetRoom(player.CurrentRoom);

                    var data = {};
                    data['State'] = 'IncetanceCard';
                    data['Index'] = card['CardIndex'];
                    data['Suit'] = card['MySuit'];

                    var js = JSON.stringify(data);
                    room.SendRoom(js, this.playerList, this.server, this.utils);
                    break;
                case 'ChengeCard':
                    var card = JSON.parse(json['Method']);
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);
                    var room = this.roomList.GetRoom(player.CurrentRoom);

                    room.ResetNotCard(this.playerList);
                    
                    var data = {};
                    data['State'] = 'ChengeCard';
                    data['BeforeIndex'] = card['BeforeIndex'];
                    data['BeforeSuit'] = card['BeforeSuit'];
                    data['AftereIndex'] = card['AfterIndex'];
                    data['AfterSuit'] = card['AfterSuit'];

                    var js = JSON.stringify(data);
                    room.SendRoom(js, this.playerList, this.server, this.utils);
                    break;

                case 'NotCard':
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);
                    var room = this.roomList.GetRoom(player.CurrentRoom);

                    player.IsNotCard = true;

                    room.NotCard(this.playerList, this.server, this.utils);
                    break;
                case 'WinChack':
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);
                    var room = this.roomList.GetRoom(player.CurrentRoom);

                    if (room.PlayerA == playerId['PlayerID']) {
                        var data = {};
                        data['State'] = 'CheakWin';

                        var js = JSON.stringify(data);
                        this.utils.SendJson(js, room.PlayerA.PlayerPort, room.PlayerA.PlayerAddres, this.server);
                    } else {
                        var data = {};
                        data['State'] = 'CheakWin';

                        var js = JSON.stringify(data);
                        this.utils.SendJson(js, room.PlayerB.PlayerPort, room.PlayerB.PlayerAddres, this.server);
                    }
                    break;
                case 'SendWin':
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);
                    var room = this.roomList.GetRoom(player.CurrentRoom);

                    var data = {};
                    data['State'] = 'SendWin';

                    var js = JSON.stringify(data);
                    room.SendRoom(js, this.playerList, this.server, this.utils);
                    break;
            }
        case 3://サーバーのメソッドの実行
            switch (json['Name']) {
                case 'CreateRoom':
                    var createRoom = JSON.parse(json['Method']);
                    var room = this.roomList.CreateRoom(createRoom['RoomName'], port, address, this.Server);
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);

                    console.log(json['PlayerId']);

                    room.PlayerA = player.PlayerId;
                    player.CurrentRoom = room.RoomID;//プレイヤーの現状のルームを更新する           

                    this.roomList.SendRoomList(port, address, this.server);
                    break;
                case 'EnterRoom':
                    var enterRoom = JSON.parse(json['Method']);
                    var room = this.roomList.GetRoom(enterRoom['RoomID']);
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);


                    room.RoomVisible = false;
                    room.PlayerB = player.PlayerId;
                    player.CurrentRoom = room.RoomID;//プレイヤーの現状のルームを更新する

                    room.SetColor(this.playerList, this.server, this.utils);

                    var data = {};
                    data['State'] = 'GameSceneLoad';

                    var js = JSON.stringify(data);

                    room.SendRoom(js, this.playerList, this.server, this.utils);
                    console.log(room);
                    break;
                case 'GameScene':
                    var playerId = JSON.parse(json['PlayerId']);
                    var roomId = this.playerList.GetPlayer(playerId['PlayerID']).CurrentRoom;
                    var room = this.roomList.GetRoom(roomId);

                    room.RoomPreparation++;

                    if (room.RoomPreparation >= 2) {
                        var data = {};
                        data['State'] = 'GameStart';

                        var js = JSON.stringify(data);
                        room.SendRoom(js, this.playerList, this.server, this.utils);
                    }
                case 'Refresh':
                    var playerId = JSON.parse(json['PlayerId']);
                    var player = this.playerList.GetPlayer(playerId['PlayerID']);
                    player.IsNotCard = false;
            }
    }
}


module.exports = Sort;

