var io = require('socket.io')(process.env.PORT || 52300);

//Custom Classes
var Player = require('./Classes/Player.js');


var admin = require("firebase-admin");

var serviceAccount = require("./clubrpg-69-firebase-adminsdk-a15p5-fcbc6997d7.json");

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://clubrpg-69.firebaseio.com"
});
console.log('Server Started');

var players = [];
var sockets = [];

var ExpHigh = 300;
var ExpLow = 100;

var db = admin.database();
var ref = db.ref("database/firestore/data");
var usersRef = ref.child("users");

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
    socket.emit('loadData', player);

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
        
        for(var i = 0; i < players.length; i++)
        {
            usersRef.child(players[i].id.toString()).set({
                username: player.username.toString(),
                playerStats: player.playerStats
            });
        }   

        socket.broadcast.emit('loadData', player);
    });

    //save info to database
    socket.on('saveData', function(data) {
        console.log('woo'); 
        for(var i = 0; i < 6; i ++)
        {
            player.playerStats.skills[i] = data.playerStats.skills[i];
            
        }
        usersRef.child(player.id.toString()).update({
                username: player.username.toString(),
                playerStats: player.playerStats
        });
        socket.broadcast.emit('saveData', player);
    });


    socket.on('successfulMinigame', function(data)
    {
        //recieve minigame type, send back random winnings (exp)
        console.log('woo'); 
        player.minigameWon = data.minigameWon;
        player.expAward = Math.random() * (ExpHigh - ExpLow) + ExpLow;
        socket.broadcast.emit('successfulMinigame', player);
        player.expAward = 0;
    });

    socket.on('disconnect', function(){
        console.log('Player disconnected');
        delete players[thisPlayerID];
        delete sockets[thisPlayerID];
        socket.broadcast.emit('disconnected', player);
    });
});