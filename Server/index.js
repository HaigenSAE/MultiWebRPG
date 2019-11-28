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
        var tryLoginID = data.loginID.toString();
        usersRef.doc(tryLoginID.toString()).get().then(doc => {
            if(!doc.exists)
            {
                console.log('no such player exists, spawning new');
                socket.emit('registered', {id: thisPlayerID})
            }
            else
            {
                console.log('attempting to load: ', tryLoginID);
                var myData = doc.data();
                delete players[thisPlayerID];
                delete sockets[thisPlayerID];
                //setup new player
                players[tryLoginID] = player;
                sockets[tryLoginID] = socket;
                thisPlayerID = tryLoginID;
                //assign data
                player = myData;
                player.id = tryLoginID;
                console.log('loaded player: ', player);
                socket.emit('loggedIn', {id: thisPlayerID});
            }  
        })
        .catch(err => {
            console.log('error getting doc', err);
        });
        
        
    });

    //register new user
    socket.on('registerClient', function(data){
        var regoID = data.regoID.toString();
        usersRef.doc(regoID.toString()).get().then(doc => {
            if(!doc.exists){
                console.log('regoID = ', regoID);
                if(regoID != '')
                {
                    delete players[thisPlayerID];
                    delete sockets[thisPlayerID];
                    //setup new player
                    players[regoID] = player;
                    sockets[regoID] = socket;
                    //assign data
                    player.id = regoID;
                    thisPlayerID = regoID;
                    usersRef.doc(regoID.toString()).set({
                        username: player.username.toString(),
                        playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                        position: JSON.parse(JSON.stringify(player.position))
                    });          
                }
                else
                {
                    usersRef.doc(player.id.toString()).set({
                        username: player.username.toString(),
                        playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                        position: JSON.parse(JSON.stringify(player.position))
                    });
                }
                console.log('Created player: ', player);         
                socket.emit('registered', {id: thisPlayerID});
            }
            else
            {
                console.log(regoID , 'already exists');
                socket.emit('alreadyExists');
            }
        });
        
    });

    //bring them in
    socket.on('enterGame', function(data){
        console.log('entering game');
        //Tell the client that this is our id for the server
        socket.emit('register', {id: thisPlayerID});
        console.log(player);
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
        console.log(data); 
        for(var i = 0; i < 6; i ++)
        {
            player.playerStats.skills[i] = data.playerStats.skills[i];
            
        }
        usersRef.doc(player.id.toString()).set({
                username: player.username.toString(),
                playerStats: JSON.parse(JSON.stringify(player.playerStats)),
                position: JSON.parse(JSON.stringify(player.position))
        });
        socket.emit('saveData', player);
    });


    socket.on('successfulMinigame', function(data){
        //recieve minigame type, send back random winnings (exp)
        console.log('successful Minigame'); 
        player.minigameWon = data.minigameWon;
        player.expAward = Math.random() * (ExpHigh - ExpLow) + ExpLow;
        console.log(player.expAward, ' exp for ', player.minigameWon);
        socket.emit('successMinigame', player);
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