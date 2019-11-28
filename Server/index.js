var io = require('socket.io')(process.env.PORT || 52300);

//Custom Classes
var Player = require('./Classes/Player.js');

var admin = require("firebase-admin");
var serviceAccount = require("./ServiceKey.json");

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://clubrpg-69.firebaseio.com"
});

console.log('Server Started');

var players = [];
var sockets = [];

var ExpHigh = 300;
var ExpLow = 100;

var db = admin.firestore();
var usersRef = db.collection('users');

io.on('connection', function(socket){
    console.log('Connection Made');
    var player = new Player();
    var thisPlayerID = player.id;

    players[thisPlayerID] = player;
    sockets[thisPlayerID] = socket;

    //login using existing id
    socket.on('loginClient', function(data){
        console.log('trying to load ', + data, ' from db'); 
        var tryLoginID = data.loginID;
        usersRef.doc(tryLoginID.toString()).get().then(doc => {
            if(!doc.exists)
            {
                console.log('no such player exists');
            }
            var myData = doc.data();
            //setup new player
            player = new Player();
            player.id = tryLoginID;
            players[tryLoginID] = player;
            sockets[tryLoginID] = socket;
            thisPlayerID = tryLoginID;
            //assign data
            player = myData;
            console.log('loaded player: ', player.id);
        })
        .catch(err => {
            console.log('error getting doc', err);
        });
        
        socket.emit('register', {id: thisPlayerID})
    });

    //register new user
    socket.on('registerClient', function(data){
        
    });

    //bring them in
    socket.on('enterGame', function(data){
        //Tell the client that this is our id for the server
        socket.emit('register', {id: thisPlayerID});
        socket.emit('spawn', player); //Tell myself I have spawned
        socket.broadcast.emit('spawn', player); //Tell others I have spawned
    });
    
    

    //Load players and playerinfo
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

    //pull info from database
    socket.on('loadData', function(data) {
        console.log('load from db'); 
        usersRef.doc(player.id.toString()).get().then(doc => {
            if(!doc.exists)
            {
                console.log('no such doc exists');
            }
            var myData = doc.data();
            player.playerStats = myData.playerStats;
        })
        .catch(err => {
            console.log('error getting doc', err);
        });

        
        socket.broadcast.emit('loadData', player);
    });

    //save info to database
    socket.on('saveData', function(data) {
        console.log('save to db'); 
        for(var i = 0; i < 6; i ++)
        {
            player.playerStats.skills[i] = data.playerStats.skills[i];
            
        }
        usersRef.doc(player.id.toString()).set({
                username: player.username.toString(),
                playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                position: JSON.parse(JSON.stringify(player.position))
        });
        socket.broadcast.emit('saveData', player);
    });


    socket.on('successfulMinigame', function(data)
    {
        //recieve minigame type, send back random winnings (exp)
        console.log('successful Minigame'); 
        player.minigameWon = data.minigameWon;
        player.expAward = Math.random() * (ExpHigh - ExpLow) + ExpLow;
        socket.broadcast.emit('successMinigame', player);
        console.log('broadcast sent to client');
        player.expAward = 0;
    });

    socket.on('disconnect', function(){
        console.log('Player disconnected');
        delete players[thisPlayerID];
        delete sockets[thisPlayerID];
        socket.broadcast.emit('disconnected', player);
    });
});