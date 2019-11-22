var io = require('socket.io')(process.env.PORT || 52300);

//Custom Classes
var Player = require('./Classes/Player.js');

console.log('Server Started');

var players = [];
var sockets = [];

io.on('connection', function(socket){
    console.log('Connection Made');

    var player = new Player();
    var thisPlayerID = player.id;

    players[thisPlayerID] = player;
    sockets[thisPlayerID] = socket;

    //Tell the client that this is our id for the server
    socket.emit('register', {id: thisPlayerID});
    socket.emit('spawn', player); //Tell myself I have spawned
    socket.broadcast.emit('spawn', player); //Tell others I have spawned
    //socket.emit('loadData', player);

    //Tell myself about everyone else
    for(var playerID in players){
        if(playerID != thisPlayerID){
            socket.emit('spawn', players[playerID]);
        }
    }

    //Positional Data from Client
    socket.on('updatePosition', function(data) {
        player.position.x = data.position.x;
        player.position.y = data.position.y;
        player.position.z = data.position.z;

        socket.broadcast.emit('updatePosition', player);
    });

    //pull info from database, if none, make new
    socket.on('loadData', function(data) {
        console.log('woo'); 
        for(var i = 0; i < 6; i ++)
        {
            player.playerStats.skills[i] = data.playerStats.skills[i];
            
        }

        socket.broadcast.emit('loadData', player);
    });

    socket.on('successfulMinigame', function(data)
    {
        //recieve minigame type, send back random winnings (exp)
        console.log('woo'); 
        
    });

    socket.on('disconnect', function(){
        console.log('Player disconnected');
        delete players[thisPlayerID];
        delete sockets[thisPlayerID];
        socket.broadcast.emit('disconnected', player);
    });
});